using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fizz6.Data
{
    public interface IValueBindableSubscription : Bindings.ISubscription
    {}

    public interface IValueBindableConsumerSubscription : IValueBindableSubscription
    {
        void ValueChanged(IValueBindable bindable);
    }
        
    public interface IValueBindableProviderSubscription : IValueBindableSubscription
    {
        event Action ValueChangedEvent;
    }

    public interface IValueBindableProviderSubscription<out T> : IValueBindableProviderSubscription
    {
        T Value { get; }
    }
    
    public interface IValueBindable : IBindable
    {
        event Action ValueChangedEvent;
        public void ValueChanged(IValueBindableProviderSubscription valueChangedProvider);
    }

    public interface IValueBindable<T> : IValueBindable
    {
        public Task<T> Task { get; }
        public T Value { get; }
    }

    public class ValueBindable<T> : Bindable, IValueBindable<T>
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

                _taskCompletionSource = _taskCompletionSource is { Task: { IsCompleted: false } }
                    ? _taskCompletionSource
                    : new();

                _taskCompletionSource.SetResult(value);
                ValueChangedEvent?.Invoke();
            }
        }

        public event Action ValueChangedEvent;
        
        public void ValueChanged(IValueBindableProviderSubscription subscription)
        {
            if (subscription is not IValueBindableProviderSubscription<T> provider)
                return;

            Value = provider.Value;
        }

        public ValueBindable(IModel model, string memberName) : base(model, memberName) =>
            Bindings.Bind(this);

        ~ValueBindable() =>
            Bindings.Unbind(this);

        public override void Clear() =>
            _taskCompletionSource = null;

        private static bool AreEqual(T a, T b) =>
            EqualityComparer<T>.Default.Equals(a, b);
    }
}