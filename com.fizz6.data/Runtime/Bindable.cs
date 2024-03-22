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
        void Clear();
    }

    public interface IBindable<T> : IBindable
    {
        public Task<T> Task { get; }
        public T Value { get; }
    }

    public class Bindable<T> : IBindable<T>
    {
        private TaskCompletionSource<T> _taskCompletionSource;
        public Task<T> Task =>
            _taskCompletionSource != null
                ? _taskCompletionSource.Task
                : (_taskCompletionSource ??= new()).Task;

        private bool IsValid =>
            _taskCompletionSource is { Task: { IsCompleted: true } };

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

                _taskCompletionSource = _taskCompletionSource is { Task: { IsCompleted: false }}
                    ? _taskCompletionSource
                    : new();
                
                _taskCompletionSource.SetResult(value);
                ValueChangedEvent?.Invoke();
            }
        }
        
        public IModel Model { get; private set; }
        public string MemberName { get; private set; }
        
        public event Action ValueChangedEvent;

        public Bindable(IModel model, string memberName)
        {
            Model = model;
            MemberName = memberName;
            
            Bindings.Bind(this);
        }

        ~Bindable()
        {
            Bindings.Unbind(this);

            Model = null;
            MemberName = null;
        }

        public void Clear() =>
            _taskCompletionSource = null;

        private static bool AreEqual(T a, T b) =>
            EqualityComparer<T>.Default.Equals(a, b);
    }

    public class DependentBindable<T> : IBindable<T>
    {
        private TaskCompletionSource<T> _taskCompletionSource;
        public Task<T> Task =>
            _taskCompletionSource != null
                ? _taskCompletionSource.Task
                : Refresh();
        
        private bool IsValid =>
            _taskCompletionSource is { Task: { IsCompleted: true } };
        
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

                if (_taskCompletionSource is { Task: { IsCompleted: false } })
                    _taskCompletionSource.SetCanceled();

                _taskCompletionSource = new();
                _taskCompletionSource.SetResult(value);
                ValueChangedEvent?.Invoke();
            }
        }
        
        public IModel Model { get; private set; }
        public string MemberName { get; private set; }
        
        private Func<Task<T>> _refresh;
        private IReadOnlyList<IBindable> _dependencies;
        
        public event Action ValueChangedEvent;
        public event Action RefreshEvent;
        
        public DependentBindable(IModel model, string memberName, IReadOnlyList<IBindable> dependencies, Func<Task<T>> refresh)
        {
            Model = model;
            MemberName = memberName;
            
            _dependencies = dependencies;
            _refresh = refresh;
            
            foreach (var dependency in _dependencies)
                dependency.ValueChangedEvent += OnDependencyValueChanged;
            
            Bindings.Bind(this);
        }

        ~DependentBindable()
        {
            Bindings.Unbind(this);
            
            foreach (var dependency in _dependencies)
                dependency.ValueChangedEvent -= OnDependencyValueChanged;
            
            Model = null;
            MemberName = null;

            _dependencies = null;
            _refresh = null;
        }

        public void Clear() =>
            _taskCompletionSource = null;

        public Task<T> Refresh()
        {
            if (_taskCompletionSource is { Task: { IsCompleted: false } })
                _taskCompletionSource.SetCanceled();
            
            var taskCompletionSource = new TaskCompletionSource<T>();
            _taskCompletionSource = taskCompletionSource;

            RefreshEvent?.Invoke();

            var task = _refresh.Invoke();
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

        private static bool AreEqual(T a, T b) =>
            EqualityComparer<T>.Default.Equals(a, b);
    }

    public interface IInvokable
    {
        void Invoke();
    }

    public class InvokableBindable : IBindable, IInvokable
    {
        public IModel Model { get; private set; }
        public string MemberName { get; private set; }

        private Action _invoke;
        
        public event Action ValueChangedEvent;

        public InvokableBindable(IModel model, string memberName, Action invoke)
        {
            Model = model;
            MemberName = memberName;
            
            _invoke = invoke;
            
            Bindings.Bind(this);
        }
        
        ~InvokableBindable()
        {
            Bindings.Unbind(this);

            Model = null;
            MemberName = null;

            _invoke = null;
        }

        public void Clear()
        {}

        public void Invoke() =>
            _invoke?.Invoke();
    }
}