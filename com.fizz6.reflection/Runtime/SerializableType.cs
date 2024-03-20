using System;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableType : IEquatable<SerializableType>, IEquatable<Type>
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
                module = value.Module;
                fullName = value.FullName;
                _value = value;
            }
        }

        public SerializableType(Type value) =>
            Value = value;

        public static implicit operator SerializableType(Type value) =>
            new(value);

        public static implicit operator Type(SerializableType value) =>
            value.Value;

        public bool Equals(SerializableType other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Value == other.Value;
        }

        public bool Equals(Type other) =>
            Value == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((SerializableType)obj);
        }

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
