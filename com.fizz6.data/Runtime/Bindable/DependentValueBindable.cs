using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fizz6.Data
{
    public class DependentValueBindable<TOut> : Bindable, IValueBindable<TOut>
    {
        private TaskCompletionSource<TOut> _taskCompletionSource;
        public Task<TOut> Task =>
            _taskCompletionSource != null
                ? _taskCompletionSource.Task
                : Refresh();
        
        private bool IsValid =>
            _taskCompletionSource is { Task: { IsCompleted: true } };
        
        public TOut Value => 
            IsValid
                ? _taskCompletionSource.Task.Result
                : default;
        
        protected IReadOnlyList<IValueBindable> Dependencies { get; set; }
        protected Func<Task<TOut>> RefreshCallback { get; set; }
        
        public event Action ValueChangedEvent;
        
        public void ValueChanged(IValueBindableProviderSubscription subscription)
        {}

        public event Action RefreshEvent;
        
        public DependentValueBindable(IModel model, string memberName, IReadOnlyList<IValueBindable> dependencies, Func<Task<TOut>> refreshCallback) : base(model, memberName)
        {
            Dependencies = dependencies;
            RefreshCallback = refreshCallback;
            
            foreach (var dependency in Dependencies)
                dependency.ValueChangedEvent += OnDependencyValueChanged;
            
            Bindings.Bind(this);
        }

        ~DependentValueBindable()
        {
            Bindings.Unbind(this);
            
            foreach (var dependency in Dependencies)
                dependency.ValueChangedEvent -= OnDependencyValueChanged;

            Dependencies = null;
            RefreshCallback = null;
        }

        public override void Clear() =>
            _taskCompletionSource = null;

        public Task<TOut> Refresh()
        {
            if (_taskCompletionSource is { Task: { IsCompleted: false } })
                _taskCompletionSource.SetCanceled();
            
            var taskCompletionSource = new TaskCompletionSource<TOut>();
            _taskCompletionSource = taskCompletionSource;

            RefreshEvent?.Invoke();

            var task = RefreshCallback.Invoke();
            task.ContinueWith(
                async _ =>
                {
                    if (taskCompletionSource.Task.IsCanceled)
                        return;
                    
                    var value = await task;
                    taskCompletionSource.SetResult(value);
                    ValueChangedEvent?.Invoke();
                }
            );
            
            return _taskCompletionSource.Task;
        }

        private void OnDependencyValueChanged() =>
            Refresh();
    }

    public class DependentValueBindable<TIn0, TOut> : DependentValueBindable<TOut>
    {
        private readonly Func<TIn0, Task<TOut>> _refreshCallback;
        
        public DependentValueBindable(IModel model, string memberName, IReadOnlyList<IValueBindable> dependencies, Func<TIn0, Task<TOut>> refreshCallback)
            : base(model, memberName, dependencies, null)
        {
            _refreshCallback = refreshCallback;
            RefreshCallback = Execute;
        }

        private async Task<TOut> Execute()
        {
            if (Dependencies[0] is not IValueBindable<TIn0> arg0)
                return default;
            
            var task0 = arg0.Task;
            
            var tasks = new Task[] { task0 };
            await System.Threading.Tasks.Task.WhenAll(tasks);
            
            return await _refreshCallback.Invoke(task0.Result);
        }
    }
    
    public class DependentValueBindable<TIn0, TIn1, TOut> : DependentValueBindable<TOut>
    {
        private readonly Func<TIn0, TIn1, Task<TOut>> _refreshCallback;
        
        public DependentValueBindable(IModel model, string memberName, IReadOnlyList<IValueBindable> dependencies, Func<TIn0, TIn1, Task<TOut>> refreshCallback)
            : base(model, memberName, dependencies, null)
        {
            _refreshCallback = refreshCallback;
            RefreshCallback = Execute;
        }

        private async Task<TOut> Execute()
        {
            if (Dependencies[0] is not IValueBindable<TIn0> arg0 ||
                Dependencies[1] is not IValueBindable<TIn1> arg1)
                return default;
            
            var task0 = arg0.Task;
            var task1 = arg1.Task;
            
            var tasks = new Task[] { task0, task1 };
            await System.Threading.Tasks.Task.WhenAll(tasks);
            
            return await _refreshCallback.Invoke(task0.Result, task1.Result);
        }
    }
}