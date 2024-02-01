using System;
using System.Reflection;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableMethodInfo : SerializableMemberInfo
    {
        public new MethodInfo Value
        {
            get => base.Value as MethodInfo;
            set => base.Value = value;
        }

        public SerializableMethodInfo()
        {}

        public SerializableMethodInfo(MethodInfo value) : base(value) 
        {}
            
        public static implicit operator SerializableMethodInfo(MethodInfo value) => 
            new(value);

        public static implicit operator MethodInfo(SerializableMethodInfo value) =>
            value.Value;
    }
}