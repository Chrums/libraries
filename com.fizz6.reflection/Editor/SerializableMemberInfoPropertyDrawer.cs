using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fizz6.Core;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableMemberInfo))]
    public class SerializableMemberInfoPropertyDrawer : PropertyDrawer
    {
        private class MemberInfoItem
        {
            public Type Type { get; }
            public MemberInfo MemberInfo { get; }

            public MemberInfoItem(Type type, MemberInfo memberInfo)
            {
                Type = type;
                MemberInfo = memberInfo;
            }
        }
        
        protected virtual IEnumerable<Type> Types => ReflectionConfig.Instance.Types;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var serializableMemberInfo = property.GetValue<SerializableMemberInfo>();

            if (serializableMemberInfo == null)
                return;
            
            var buttonPosition = EditorGUI.PrefixLabel(position, label);
            var content = GetGUIContent(serializableMemberInfo.Value);

            if (!GUI.Button(buttonPosition, content, EditorStyles.popup)) 
                return;

            var memberInfosItems = Types.SelectMany(GetMemberInfoItems);
            var dropdown =
                new SearchableDropdown<MemberInfo>(
                    new SearchableDropdown<MemberInfo>.State(),
                    nameof(MemberInfo),
                    memberInfosItems
                        .Select(memberInfoItem => new SearchableDropdown<MemberInfo>.Item(memberInfoItem.MemberInfo, $"{memberInfoItem.MemberInfo.Module.Assembly}/{memberInfoItem.Type.Name}/{memberInfoItem.MemberInfo.DeclaringType?.Name}.{GetMemberInfoName(memberInfoItem.MemberInfo)}"))
                        .ToArray(),
                    memberInfo => serializableMemberInfo.Value = memberInfo
                );

            dropdown.Show(buttonPosition);
        }

        protected virtual GUIContent GetGUIContent(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return new GUIContent();

            var memberInfoName = GetMemberInfoName(memberInfo);

            var text = $"{memberInfo.DeclaringType?.GetFriendlyName() ?? "?"}.{memberInfoName}";
            var tooltip = memberInfo.MemberType switch
            {
                MemberTypes.Field => memberInfo is FieldInfo fieldInfo 
                    ? GetFieldInfoTooltip(fieldInfo) 
                    : string.Empty,
                MemberTypes.Property => memberInfo is PropertyInfo propertyInfo 
                    ? GetPropertyInfoTooltip(propertyInfo) 
                    : string.Empty,
                MemberTypes.Method => memberInfo is MethodInfo methodInfo
                    ? GetMethodInfoTooltip(methodInfo)
                    : string.Empty,
                _ => string.Empty
            };

            return new GUIContent(text, tooltip);
        }

        private static string GetFieldInfoTooltip(FieldInfo fieldInfo)
        {
            var name = fieldInfo.Name;
            var fieldTypeName = fieldInfo.FieldType.GetFriendlyName();
            var accessibility = fieldInfo.IsPublic
                ? nameof(FieldInfo.IsPublic)
                : nameof(FieldInfo.IsPrivate);
            return $"{nameof(FieldInfo)}\n{nameof(FieldInfo.Name)}: {name}\n{nameof(FieldInfo.FieldType)}: {fieldTypeName}\nAccessibility: {accessibility}";
        }

        private static string GetPropertyInfoTooltip(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            var propertyTypeName = propertyInfo.PropertyType.GetFriendlyName();
            
            var getter = propertyInfo.GetGetMethod();
            var getAccessibility = getter == null 
                ? "None" 
                : getter.IsPublic
                    ? nameof(MethodInfo.IsPublic)
                    : nameof(MethodInfo.IsPrivate);
            var setter = propertyInfo.GetSetMethod();
            var setAccessibility = setter == null
                ? "None"
                : setter.IsPublic
                    ? nameof(MethodInfo.IsPublic)
                    : nameof(MethodInfo.IsPrivate);
            
            return $"{nameof(PropertyInfo)}\n{nameof(PropertyInfo.Name)}: {name}\n{nameof(PropertyInfo.PropertyType)}: {propertyTypeName}\nAccessibility: [ Get: {getAccessibility}, Set: {setAccessibility} ]";
        }

        private static string GetMethodInfoTooltip(MethodInfo methodInfo)
        {
            var name = methodInfo.Name;
            var returnTypeName = methodInfo.ReturnType.GetFriendlyName();
            var accessibility = methodInfo.IsPublic
                ? nameof(MethodInfo.IsPublic)
                : nameof(MethodInfo.IsPrivate);
            return $"{nameof(MethodInfo)}\n{nameof(MethodInfo.Name)}: {name}\n{nameof(MethodInfo.ReturnType)}: {returnTypeName}\nAccessibility: {accessibility}";
        }

        protected virtual string GetMemberInfoName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return string.Empty;
            
            var memberName = memberInfo.Name;
            if (memberInfo is not MethodInfo methodInfo) 
                return memberName;
            
            var parameters = 
                string.Join(
                    ", ", 
                    methodInfo
                        .GetParameters()
                        .Select(parameterInfo => $"{parameterInfo.ParameterType.GetFriendlyName()} {parameterInfo.Name}")
                );

            return $"{memberName}({parameters})";
        }

        private IEnumerable<MemberInfoItem> GetMemberInfoItems(Type type) =>
            GetMemberInfos(type)
                .Select(memberInfo => new MemberInfoItem(type, memberInfo));

        protected virtual IEnumerable<MemberInfo> GetMemberInfos(Type type) =>
            type.GetMembers()
                .Where(memberInfo => memberInfo.MemberType is MemberTypes.Field or MemberTypes.Property or MemberTypes.Method);
    }
}