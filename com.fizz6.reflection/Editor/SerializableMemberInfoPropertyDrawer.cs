using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableMemberInfo))]
    public class SerializableMemberInfoPropertyDrawer : PropertyDrawer
    {
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

            var memberInfos = Types.SelectMany(GetMemberInfos);
            var dropdown =
                new SearchableDropdown<MemberInfo>(
                    new SearchableDropdown<MemberInfo>.State(),
                    nameof(MemberInfo),
                    memberInfos
                        .Select(memberInfo => new SearchableDropdown<MemberInfo>.Item(memberInfo, $"{memberInfo.Module.Assembly}/{memberInfo.DeclaringType}/{GetMemberInfoName(memberInfo)}"))
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
            
            var text = $"{memberInfo.DeclaringType?.Name ?? "?"}.{memberInfoName}";
            return new GUIContent(text);
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
                        .Select(parameterInfo => $"{parameterInfo.ParameterType} {parameterInfo.Name}")
                );

            return $"{memberName}({parameters})";
        }

        protected virtual IEnumerable<MemberInfo> GetMemberInfos(Type type) =>
            type.GetMembers();
    }
}