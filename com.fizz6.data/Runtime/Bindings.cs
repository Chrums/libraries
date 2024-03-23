using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Data
{
    public static class Bindings
    {
        public interface ISubscription
        {}
        
        private class Source : IEquatable<Source>
        {
            private readonly string _identifier;
            
            public Source(IBindable bindable)
            {
                var gameObjectIdentifier = bindable.Model.Component.gameObject
                    .GetInstanceID()
                    .ToString();
                var componentTypeIdentifier = bindable.Model.Component.GetType().Name;
                var memberNameIdentifier = bindable.MemberName;
                _identifier = $"{gameObjectIdentifier}-{componentTypeIdentifier}-{memberNameIdentifier}";
            }

            public Source(Component component, MemberInfo memberInfo)
            {
                var gameObjectIdentifier = component.gameObject
                    .GetInstanceID()
                    .ToString();
                var componentTypeIdentifier = component.GetType().Name;
                var memberNameIdentifier = memberInfo.Name;
                _identifier = $"{gameObjectIdentifier}-{componentTypeIdentifier}-{memberNameIdentifier}";
            }

            public bool Equals(Source other)
            {
                if (ReferenceEquals(null, other)) 
                    return false;
                
                if (ReferenceEquals(this, other)) 
                    return true;
                
                return _identifier == other._identifier;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) 
                    return false;
                
                if (ReferenceEquals(this, obj)) 
                    return true;
                
                if (obj.GetType() != this.GetType()) 
                    return false;
                
                return Equals((Source)obj);
            }

            public override int GetHashCode() =>
                _identifier != null 
                    ? _identifier.GetHashCode() 
                    : 0;

            public override string ToString() =>
                _identifier;
        }

        private class ValueBinding
        {
            private IValueBindable _bindable;
            private readonly Dictionary<IValueBindableProviderSubscription, Action> _providerOnValueChangedActions = new();
            public event Action<IValueBindable> BindableValueChangedEvent;

            public void Bind(IValueBindable bindable)
            {
                _bindable = bindable;
                _bindable.ValueChangedEvent += OnBindableValueChanged;
            }

            public void Unbind()
            {
                _bindable.ValueChangedEvent -= OnBindableValueChanged;
                _bindable = null;
            }

            public void Subscribe(IValueBindableSubscription subscription)
            {
                if (subscription is IValueBindableConsumerSubscription valueBindableConsumerSubscription)
                {
                    BindableValueChangedEvent += valueBindableConsumerSubscription.ValueChanged;
                }

                if (subscription is IValueBindableProviderSubscription valueBindableProviderSubscription)
                {
                    void OnValueChanged() =>
                        _bindable.ValueChanged(valueBindableProviderSubscription);
                    
                    _providerOnValueChangedActions.TryAdd(valueBindableProviderSubscription, OnValueChanged);
                    valueBindableProviderSubscription.ValueChangedEvent += OnValueChanged;
                }
            }

            public void Unsubscribe(IValueBindableSubscription subscription)
            {
                if (subscription is IValueBindableConsumerSubscription valueBindableConsumerSubscription)
                {
                    BindableValueChangedEvent -= valueBindableConsumerSubscription.ValueChanged;
                }
                
                if (subscription is IValueBindableProviderSubscription valueBindableProviderSubscription)
                {
                    if (!_providerOnValueChangedActions.TryGetValue(valueBindableProviderSubscription, out var onValueChanged))
                        return;

                    _providerOnValueChangedActions.Remove(valueBindableProviderSubscription);
                    valueBindableProviderSubscription.ValueChangedEvent -= onValueChanged;
                }
            }

            private void OnBindableValueChanged() =>
                BindableValueChangedEvent?.Invoke(_bindable);
        }
        
        private class InvokableBinding
        {
            private IInvokableBindable _bindable;

            public void Bind(IInvokableBindable bindable) =>
                _bindable = bindable;

            public void Unbind() =>
                _bindable = null;

            public void Subscribe(IInvokableBindableSubscription subscription) =>
                subscription.InvokeEvent += _bindable.Invoke;

            public void Unsubscribe(IInvokableBindableSubscription subscription) =>
                subscription.InvokeEvent -= _bindable.Invoke;
        }
        
        private static readonly Dictionary<Source, ValueBinding> ValueBindings = new();

        public static void Bind(IValueBindable bindable)
        {
            var source = new Source(bindable);
            if (!ValueBindings.TryGetValue(source, out var binding))
            {
                binding = new ValueBinding();
                ValueBindings[source] = binding;
            }
            
            binding.Bind(bindable);
        }

        public static void Unbind(IValueBindable bindable)
        {
            var source = new Source(bindable);
            if (!ValueBindings.TryGetValue(source, out var binding))
                return;
            
            binding.Unbind();
        }

        public static void Subscribe(Component component, MemberInfo memberInfo, IValueBindableSubscription subscription)
        {
            var source = new Source(component, memberInfo);
            if (!ValueBindings.TryGetValue(source, out var binding))
            {
                binding = new ValueBinding();
                ValueBindings[source] = binding;
            }
            
            binding.Subscribe(subscription);
        }

        public static void Unsubscribe(Component component, MemberInfo memberInfo, IValueBindableSubscription subscription)
        {
            var source = new Source(component, memberInfo);
            if (!ValueBindings.TryGetValue(source, out var binding))
                return;
            
            binding.Unsubscribe(subscription);
        }
        
        private static readonly Dictionary<Source, InvokableBinding> InvokableBindings = new();

        public static void Bind(IInvokableBindable bindable)
        {
            var source = new Source(bindable);
            if (!InvokableBindings.TryGetValue(source, out var binding))
            {
                binding = new InvokableBinding();
                InvokableBindings[source] = binding;
            }
            
            binding.Bind(bindable);
        }

        public static void Unbind(IInvokableBindable bindable)
        {
            var source = new Source(bindable);
            if (!InvokableBindings.TryGetValue(source, out var binding))
                return;
            
            binding.Unbind();
        }

        public static void Subscribe(Component component, MemberInfo memberInfo, IInvokableBindableSubscription subscription)
        {
            var source = new Source(component, memberInfo);
            if (!InvokableBindings.TryGetValue(source, out var binding))
            {
                binding = new InvokableBinding();
                InvokableBindings[source] = binding;
            }
            
            binding.Subscribe(subscription);
        }

        public static void Unsubscribe(Component component, MemberInfo memberInfo, IInvokableBindableSubscription subscription)
        {
            var source = new Source(component, memberInfo);
            if (!InvokableBindings.TryGetValue(source, out var binding))
                return;
            
            binding.Unsubscribe(subscription);
        }
    }
}
