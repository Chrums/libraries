using System;
using System.Reflection;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializablePropertyInfo : SerializableMemberInfo
    {
        public new PropertyInfo Value
        {
            get => base.Value as PropertyInfo;
            set => base.Value = value;
        }

        public SerializablePropertyInfo()
        {}

        public SerializablePropertyInfo(PropertyInfo value) : base(value) 
        {}
            
        public static implicit operator SerializablePropertyInfo(PropertyInfo value) => 
            new(value);

        public static implicit operator PropertyInfo(SerializablePropertyInfo value) =>
            value.Value;
    }
}