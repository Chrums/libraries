using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableMethodInfo))]
    public class SerializableMethodInfoPropertyDrawer : SerializableMemberInfoPropertyDrawer
    {
        private static IEnumerable<MethodInfo> GetMethodInfos(Type type) =>
            type.GetMethods(ReflectionConfig.Instance.BindingFlags)
                .Where(methodInfo => !methodInfo.IsSpecialName);
        
        protected override IEnumerable<Type> Types =>
            base.Types.Where(type => GetMethodInfos(type).Any());

        protected override IEnumerable<MemberInfo> GetMemberInfos(Type type) =>
            GetMethodInfos(type);
    }
}