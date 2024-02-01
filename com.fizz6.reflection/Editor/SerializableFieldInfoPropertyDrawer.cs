using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableFieldInfo))]
    public class SerializableFieldInfoPropertyDrawer : SerializableMemberInfoPropertyDrawer
    {
        private static IEnumerable<FieldInfo> GetFieldInfos(Type type) =>
            type.GetFields(ReflectionConfig.Instance.BindingFlags)
                .Where(fieldInfo => !fieldInfo.IsSpecialName);
        
        protected override IEnumerable<Type> Types => 
            base.Types.Where(type => GetFieldInfos(type).Any());
        
        protected override IEnumerable<MemberInfo> GetMemberInfos(Type type) =>
            type.GetFields();
    }
}