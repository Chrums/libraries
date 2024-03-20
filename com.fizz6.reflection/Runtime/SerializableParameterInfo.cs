using System;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableParameterInfo : IEquatable<SerializableParameterInfo>, IEquatable<ParameterInfo>
    {
        [SerializeField] 
        private SerializableMethodInfo serializableMethodInfo;

        [SerializeField] 
        private int position;
        
        [NonSerialized] 
        private ParameterInfo _value;
        public ParameterInfo Value
        {
            get
            {
                if (serializableMethodInfo.Value == null)
                    return null;
                
                if (_value != null) 
                    return _value;

                var parameters = serializableMethodInfo.Value.GetParameters();
                if (position >= parameters.Length)
                    return null;
                
                _value = parameters[position];
                return _value;
            }

            set
            {
                serializableMethodInfo = value.Member as MethodInfo;
                position = _value.Position;
                _value = value;
            }
        }

        public SerializableParameterInfo(ParameterInfo value) =>
            Value = value;

        public static implicit operator SerializableParameterInfo(ParameterInfo value) =>
            new(value);

        public static implicit operator ParameterInfo(SerializableParameterInfo value) =>
            value.Value;

        public bool Equals(SerializableParameterInfo other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Value == other.Value;
        }

        public bool Equals(ParameterInfo other) =>
            Value == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((SerializableParameterInfo)obj);
        }

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
