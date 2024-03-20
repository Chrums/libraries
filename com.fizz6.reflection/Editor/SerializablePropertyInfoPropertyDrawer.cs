using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fizz6.Core;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializablePropertyInfo))]
    public class SerializablePropertyInfoPropertyDrawer : SerializableMemberInfoPropertyDrawer
    {
        private static IEnumerable<PropertyInfo> GetPropertyInfos(Type type) =>
            type.GetProperties(ReflectionConfig.Instance.BindingFlags)
                .Where(propertyInfo => !propertyInfo.IsSpecialName);
        
        protected override IEnumerable<Type> Types =>
            base.Types.Where(type => GetPropertyInfos(type).Any());
        
        protected override IEnumerable<MemberInfo> GetMemberInfos(Type type) =>
            type.GetProperties();
    }
}