using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableAssembly : IEquatable<SerializableAssembly>, IEquatable<Assembly>
    {
        [SerializeField]
        private string fullName;
        
        [NonSerialized]
        private Assembly _value;
        public Assembly Value
        {
            get
            {
                if (string.IsNullOrEmpty(fullName))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(assembly => assembly.FullName == fullName);
                return _value;
            }
            
            set
            {
                fullName = value.FullName;
                _value = value;
            }
        }

        public SerializableAssembly(Assembly value) =>
            Value = value;

        public static implicit operator SerializableAssembly(Assembly value) =>
            new(value);

        public static implicit operator Assembly(SerializableAssembly value) =>
            value.Value;

        public bool Equals(SerializableAssembly other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Value == other.Value;
        }

        public bool Equals(Assembly other) =>
            Value == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((SerializableAssembly)obj);
        }

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
