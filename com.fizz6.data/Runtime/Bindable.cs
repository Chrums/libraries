using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fizz6.Data
{
    public interface IBindable
    {
        public IModel Model { get; }
        public string MemberName { get; }
        event Action ValueChangedEvent;
        IBindable Bind(IModel model, string memberName);
        IBindable Unbind();
        void Clear();
    }

    public interface IBindable<T> : IBindable
    {
        public Task<T> Task { get; }
    }

    public class Bindable<T> : IBindable<T>
    {
        private TaskCompletionSource<T> _taskCompletionSource;
        public virtual Task<T> Task =>
            _taskCompletionSource == null
                ? (_taskCompletionSource ??= new TaskCompletionSource<T>()).Task
                : _taskCompletionSource.Task;

        public bool IsValid => 
            _taskCompletionSource != null && _taskCompletionSource.Task.IsCompleted;

        public T Value
        {
            get => 
                IsValid
                    ? _taskCompletionSource.Task.Result
                    : default;
            
            set
            {
                if (IsValid && AreEqual(_taskCompletionSource.Task.Result, value))
                    return;

                _taskCompletionSource = _taskCompletionSource == null || _taskCompletionSource.Task.IsCompleted
                    ? new TaskCompletionSource<T>()
                    : _taskCompletionSource;
                
                _taskCompletionSource.SetResult(value);
                ValueChangedEvent?.Invoke();
            }
        }
        
        public IModel Model { get; private set; }
        public string MemberName { get; private set; }
        
        public event Action ValueChangedEvent;

        public virtual IBindable Bind(IModel model, string memberName)
        {
            Model = model;
            MemberName = memberName;
            Bindings.Bind(this);
            return this;
        }

        public virtual IBindable Unbind()
        {
            Bindings.Unbind(this);
            return this;
        }
        
        public virtual void Clear()
        {}

        protected static bool AreEqual(T a, T b) =>
            EqualityComparer<T>.Default.Equals(a, b);
    }

    public class DependentBindable<T> : Bindable<T>
    {
        private TaskCompletionSource<T> _taskCompletionSource;
        public override Task<T> Task =>
            IsValid
                ? base.Task 
                : Refresh();

        private Func<Task<T>> _refresh;
        private IReadOnlyList<IBindable> _dependencies;
        
        public event Action RefreshEvent;
        
        public IBindable Bind(IModel model, string name, Func<Task<T>> refresh, IReadOnlyList<IBindable> dependencies)
        {
            Bind(model, name);
            
            _refresh = refresh;
            _dependencies = dependencies;
            
            foreach (var dependency in _dependencies)
                dependency.ValueChangedEvent += OnDependencyValueChanged;
            
            return this;
        }

        public override IBindable Unbind()
        {
            foreach (var dependency in _dependencies)
                dependency.ValueChangedEvent -= OnDependencyValueChanged;
            
            return base.Unbind();
        }

        public override void Clear()
        {
            base.Clear();
            _taskCompletionSource = null;
        }

        public Task<T> Refresh()
        {
            if (_taskCompletionSource is { Task: { IsCompleted: false } })
                return _taskCompletionSource.Task;
            
            _taskCompletionSource = new TaskCompletionSource<T>();

            RefreshEvent?.Invoke();
            
            var task = _refresh.Invoke();
            task.ContinueWith(
                async _ =>
                {
                    var value = await task;
                    _taskCompletionSource.SetResult(value);
                    Value = value;
                }
            );
            
            return _taskCompletionSource.Task;
        }

        private void OnDependencyValueChanged() =>
            Refresh();
    }
}