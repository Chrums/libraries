using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableMemberInfo
    {
        [SerializeField]
        private SerializableType type;
        
        [SerializeField]
        private string name;
        
        [SerializeField]
        private int metadataToken;
            
        [NonSerialized]
        private MemberInfo _value;
        public MemberInfo Value
        {
            get
            {
                if (type.Value == null || string.IsNullOrEmpty(name))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = type.Value
                    .GetMember(name)
                    .FirstOrDefault(memberInfo => memberInfo.MetadataToken == metadataToken);
                return _value;
            }
            
            set
            {
                if (value == null)
                {
                    type = null;
                    name = null;
                    metadataToken = default;
                    _value = null;
                    return;
                }
                
                type = value.DeclaringType;
                name = value.Name;
                metadataToken = value.MetadataToken;
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
    }
}
