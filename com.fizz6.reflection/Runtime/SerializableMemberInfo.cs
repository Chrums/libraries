using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableMemberInfo : IEquatable<SerializableMemberInfo>, IEquatable<MemberInfo>
    {
        [Serializable]
        private class MethodSignature : IEquatable<MethodInfo>
        {
            [SerializeField] 
            private SerializableType[] parameterSerializableTypes;

            public MethodSignature(MethodInfo methodInfo)
            {
                parameterSerializableTypes = methodInfo.GetParameters()
                    .Select(parameterInfo => new SerializableType(parameterInfo.ParameterType))
                    .ToArray();
            }

            public bool Equals(MethodInfo methodInfo)
            {
                if (methodInfo == null)
                    return false;

                var parameterTypes = parameterSerializableTypes
                    .Select(parameterSerializableType => parameterSerializableType.Value);
                var methodInfoParameterTypes = methodInfo.GetParameters()
                    .Select(parameterInfo => parameterInfo.ParameterType);

                return parameterTypes.SequenceEqual(methodInfoParameterTypes);
            }
        }
        
        [SerializeField]
        private SerializableType serializableType;
        
        [SerializeField]
        private string name;

        [SerializeField] 
        private MemberTypes memberType;

        [SerializeField] 
        private SerializableType[] parameterSerializableTypes;
            
        [NonSerialized]
        private MemberInfo _value;
        public MemberInfo Value
        {
            get
            {
                if (serializableType.Value == null || string.IsNullOrEmpty(name))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = memberType is MemberTypes.Field or MemberTypes.Property
                    ? serializableType.Value.GetMember(name)
                        .FirstOrDefault()
                    : serializableType.Value.GetMember(name)
                        .FirstOrDefault(memberInfo => memberInfo is MethodInfo methodInfo && CompareMethodSignature(methodInfo));
                
                return _value;
            }
            
            set
            {
                if (value == null)
                {
                    serializableType = null;
                    name = null;
                    memberType = MemberTypes.All;
                    _value = null;
                    return;
                }
                
                serializableType = value.DeclaringType;
                name = value.Name;
                memberType = value.MemberType;

                if (value is MethodInfo methodInfo)
                    parameterSerializableTypes = methodInfo.GetParameters()
                        .Select(parameterInfo => new SerializableType(parameterInfo.ParameterType))
                        .ToArray();
                
                _value = value;
            }
        }
        
        public SerializableMemberInfo()
        {}

        public SerializableMemberInfo(MemberInfo value) =>
            Value = value;
            
        public static implicit operator SerializableMemberInfo(MemberInfo value) => 
            new(value);

        public static implicit operator MemberInfo(SerializableMemberInfo value) =>
            value.Value;

        public bool Equals(SerializableMemberInfo other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Value == other.Value;
        }

        public bool Equals(MemberInfo other) =>
            Value == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((SerializableMemberInfo)obj);
        }

        public override int GetHashCode() =>
            Value.GetHashCode();

        private bool CompareMethodSignature(MethodInfo methodInfo)
        {
            var parameterTypes = parameterSerializableTypes
                .Select(parameterSerializableType => parameterSerializableType.Value);
            var otherParameterTypes = methodInfo.GetParameters()
                .Select(parameterInfo => parameterInfo.ParameterType);
            return parameterTypes.SequenceEqual(otherParameterTypes);
        }
    }
}
