using System;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableType
    {
        [SerializeField]
        private SerializableModule module;
        
        [SerializeField]
        private string fullName;
        
        [NonSerialized] 
        private Type _value;
        public Type Value
        {
            get
            {
                if (module.Value == null || string.IsNullOrEmpty(fullName))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = module.Value.GetType(fullName);
                return _value;
            }

            set
            {
                module = new SerializableModule(value.Module);
                fullName = value.FullName;
                _value = value;
            }
        }

        public SerializableType(Type value)
        {
            module = value.Module;
            fullName = value.FullName;
            _value = value;
        }

        public static implicit operator SerializableType(Type value) =>
            new(value);

        public static implicit operator Type(SerializableType value) =>
            value.Value;
    }
}
