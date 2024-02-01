using System;
using System.Linq;
using System.Reflection;
using Fizz6.Core.Editor;
using Fizz6.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Data.Editor
{
    // TODO: Clean this up...
    // Expose GetGUIContent and GetMemberInfoName on SerializableMemberInfoPropertyDrawer or make an extension class?
    
    [CustomPropertyDrawer(typeof(StringMemberInfoConsumerProvider))]
    public class StringMemberInfoConsumerProviderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var stringMemberInfoConsumerProvider = property.GetValue<StringMemberInfoConsumerProvider>();

            if (stringMemberInfoConsumerProvider.Providers == null)
                stringMemberInfoConsumerProvider.Providers = new IProvider[stringMemberInfoConsumerProvider.InputTypes.Length];
            else if (stringMemberInfoConsumerProvider.Providers.Length != stringMemberInfoConsumerProvider.InputTypes.Length)
            {
                var providers = stringMemberInfoConsumerProvider.Providers;
                Array.Resize(ref providers, stringMemberInfoConsumerProvider.InputTypes.Length);
                stringMemberInfoConsumerProvider.Providers = providers;
            }

            position = ConsumerPropertyDrawer.ProvidersOnGUI(position, property);
            
            var componentSerializedProperty = property.FindPropertyRelative("component");
            var memberInfoSerializedProperty = property.FindPropertyRelative("memberInfo");

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, componentSerializedProperty);
                position.y = position.yMax;
                if (changeCheckScope.changed)
                    memberInfoSerializedProperty.boxedValue = new SerializableMemberInfo(null);
            }

            if (componentSerializedProperty.objectReferenceValue == null)
                return;

            var component = componentSerializedProperty.objectReferenceValue as Component;
            if (!component)
                return;
            
            var type = component.GetType();
            var memberInfos = type.GetMembers()
                .Where(memberInfo => (memberInfo is MethodInfo methodInfo && methodInfo.ReturnType == typeof(string)) || 
                                     (memberInfo is FieldInfo fieldInfo && fieldInfo.FieldType == typeof(string)) ||
                                     (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType == typeof(string)));
            
            
            
            var serializableMemberInfo = memberInfoSerializedProperty.GetValue<SerializableMemberInfo>();

            var prefixLabelGUIContent = new GUIContent(memberInfoSerializedProperty.displayName);
            var buttonPosition = EditorGUI.PrefixLabel(position, prefixLabelGUIContent);
            
            var content = GetGUIContent(serializableMemberInfo.Value);
            if (!GUI.Button(buttonPosition, content, EditorStyles.popup)) 
                return;

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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // return base.GetPropertyHeight(property, label);

            return ConsumerPropertyDrawer.ProvidersGetPropertyHeight(property) + EditorGUIUtility.singleLineHeight * 2.0f;
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
    }
}
