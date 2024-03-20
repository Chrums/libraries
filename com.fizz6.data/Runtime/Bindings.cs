using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Data
{
    public static class Bindings
    {
        private class Source : IComparable<Source>
        {
            private string _identifier;
            
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

            public int CompareTo(Source other)
            {
                if (ReferenceEquals(this, other)) 
                    return 0;
                
                if (ReferenceEquals(null, other)) 
                    return 1;
                
                return string.Compare(_identifier, other._identifier, StringComparison.Ordinal);
            }

            public override string ToString()
            {
                // return base.ToString();
                return _identifier;
            }
        }

        private class Binding
        {
            public event Action ValueChangedEvent;
            private IBindable _bindable;

            public void Bind(IBindable bindable)
            {
                _bindable = bindable;
                _bindable.ValueChangedEvent += ValueChangedEvent;
            }

            public void Unbind()
            {
                _bindable.ValueChangedEvent -= ValueChangedEvent;
                _bindable = null;
            }

            public void Subscribe(Action valueChangedCallback) =>
                ValueChangedEvent += valueChangedCallback;

            public void Unsubscribe(Action valueChangedCallback) =>
                ValueChangedEvent -= valueChangedCallback;
        }
        
        private static Dictionary<Source, Binding> _bindingsBySource;

        public static void Bind(IBindable bindable)
        {
            var source = new Source(bindable);
            if (!_bindingsBySource.TryGetValue(source, out var binding))
            {
                binding = new Binding();
                _bindingsBySource[source] = binding;
            }
            
            binding.Bind(bindable);
        }

        public static void Unbind(IBindable bindable)
        {
            var source = new Source(bindable);
            if (!_bindingsBySource.TryGetValue(source, out var binding))
                return;
            
            binding.Unbind();
        }

        public static void Subscribe(Component component, MemberInfo memberInfo, Action valueChangedCallback)
        {
            var source = new Source(component, memberInfo);
            if (!_bindingsBySource.TryGetValue(source, out var binding))
            {
                binding = new Binding();
                _bindingsBySource[source] = binding;
            }
            
            binding.Subscribe(valueChangedCallback);
        }

        public static void Unsubscribe(Component component, MemberInfo memberInfo, Action valueChangedCallback)
        {
            var source = new Source(component, memberInfo);
            if (!_bindingsBySource.TryGetValue(source, out var binding))
                return;
            
            binding.Unsubscribe(valueChangedCallback);
        }
    }
}
