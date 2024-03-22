using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Data
{
    public static class Bindings
    {
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

        private class Binding
        {
            public IBindable Bindable { get; private set; }
            public event Action ValueChangedEvent;

            public void Bind(IBindable bindable)
            {
                Bindable = bindable;
                Bindable.ValueChangedEvent += OnValueChanged;
            }

            public void Unbind()
            {
                Bindable.ValueChangedEvent -= OnValueChanged;
                Bindable = null;
            }

            public void Subscribe(Action valueChangedCallback) =>
                ValueChangedEvent += valueChangedCallback;

            public void Unsubscribe(Action valueChangedCallback) =>
                ValueChangedEvent -= valueChangedCallback;

            private void OnValueChanged() =>
                ValueChangedEvent?.Invoke();
        }
        
        private static readonly Dictionary<Source, Binding> BindingsBySource = new();

        public static void Bind(IBindable bindable)
        {
            var source = new Source(bindable);
            if (!BindingsBySource.TryGetValue(source, out var binding))
            {
                binding = new Binding();
                BindingsBySource[source] = binding;
            }
            
            binding.Bind(bindable);
        }

        public static void Unbind(IBindable bindable)
        {
            var source = new Source(bindable);
            if (!BindingsBySource.TryGetValue(source, out var binding))
                return;
            
            binding.Unbind();
        }

        public static void Subscribe(Component component, MemberInfo memberInfo, Action valueChangedCallback)
        {
            var source = new Source(component, memberInfo);
            if (!BindingsBySource.TryGetValue(source, out var binding))
            {
                binding = new Binding();
                BindingsBySource[source] = binding;
            }
            
            binding.Subscribe(valueChangedCallback);
        }

        public static void Unsubscribe(Component component, MemberInfo memberInfo, Action valueChangedCallback)
        {
            var source = new Source(component, memberInfo);
            if (!BindingsBySource.TryGetValue(source, out var binding))
                return;
            
            binding.Unsubscribe(valueChangedCallback);
        }
    }
}
