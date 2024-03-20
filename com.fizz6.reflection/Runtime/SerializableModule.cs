using System;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableModule : IEquatable<SerializableModule>, IEquatable<Module>
    {
        [SerializeField]
        private SerializableAssembly serializableAssembly;
        
        [SerializeField]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                _value = null;
            }
        }

        [NonSerialized] 
        private Module _value;
        public Module Value
        {
            get
            {
                if (serializableAssembly.Value == null || string.IsNullOrEmpty(name))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = serializableAssembly.Value.GetModule(name);
                return _value;
            }

            set
            {
                serializableAssembly = value.Assembly;
                name = value.Name;
                _value = value;
            }
        }

        public SerializableModule(Module value) =>
            Value = value;
            
        public static implicit operator SerializableModule(Module value) => 
            new(value);

        public static implicit operator Module(SerializableModule value) =>
            value.Value;

        public bool Equals(SerializableModule other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Value == other.Value;
        }

        public bool Equals(Module other) =>
            Value == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((SerializableModule)obj);
        }

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
