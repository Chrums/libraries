using System;
using System.Reflection;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableFieldInfo : SerializableMemberInfo
    {
        public new FieldInfo Value
        {
            get => base.Value as FieldInfo;
            set => base.Value = value;
        }

        public SerializableFieldInfo()
        {}
        
        public SerializableFieldInfo(FieldInfo value) : base(value) 
        {}
            
        public static implicit operator SerializableFieldInfo(FieldInfo value) => 
            new(value);

        public static implicit operator FieldInfo(SerializableFieldInfo value) =>
            value.Value;
    }
}
