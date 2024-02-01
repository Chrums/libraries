using System;
using System.Collections.Generic;
using System.Linq;
using Fizz6.Core;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.SerializeImplementation.Editor
{
    [CustomPropertyDrawer(typeof(SerializeImplementationAttribute))]
    public class SerializeImplementationAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var type = fieldInfo.FieldType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                type = type.GenericTypeArguments.FirstOrDefault();
            else if (type.IsArray)
                type = type.GetElementType();
            
            var assignableTypes = type
                .GetAssignableTypes()
                .Where(assignableType => !assignableType.IsGenericType)
                .Prepend(null)
                .ToArray();
            var assignableTypeNames = assignableTypes
                .Select(assignableType => assignableType?.FullName?.Replace("+", "/"))
                .ToArray();
            var typeNames = assignableTypes
                .Select(assignableType => assignableType?.Name ?? "None")
                .ToArray();

            var managedReferenceFullTypeName = property.GetManagedReferenceFullTypeName();
            var selectedIndex = Array.IndexOf(assignableTypeNames, managedReferenceFullTypeName);
            
            var controlPosition = new Rect(
                position.x, 
                position.y, 
                position.width, 
                EditorGUIUtility.singleLineHeight
            );

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                var index = EditorGUI.Popup(controlPosition, label.text, selectedIndex, typeNames);
                if (changeCheckScope.changed)
                {
                    var changeType = assignableTypes[index];
                    property.managedReferenceValue = index > 0
                        ? Activator.CreateInstance(changeType) 
                        : null;
                }
            }

            // If there's a custom property drawer for the selected type, draw that
            // Otherwise, just draw each child serialized property
            var propertyValueType = property.GetValueType();
            if (propertyValueType != null && propertyValueType.TryGetPropertyDrawer(out var propertyDrawerType))
            {
                var propertyDrawer = (PropertyDrawer)Activator.CreateInstance(propertyDrawerType);
                controlPosition.y = controlPosition.yMax + SerializedPropertyLayout.PropertyPadding;
                controlPosition.height = propertyDrawer.GetPropertyHeight(property, label);
                propertyDrawer.OnGUI(controlPosition, property, label);
                return;
            }
            
            var childrenSerializedProperties = property
                .GetChildren();

            if (!childrenSerializedProperties.Any()) 
                return;

            controlPosition.y += SerializedPropertyLayout.PropertyPadding; // Between popup and container
            
            var containerHeight = SerializedPropertyLayout.GetContainerHeight(property);
            
            var borderPosition = new Rect(controlPosition.x, controlPosition.yMax, controlPosition.width, containerHeight);
            EditorGUI.DrawRect(borderPosition, SerializedPropertyLayout.BorderColor);

            controlPosition.x += SerializedPropertyLayout.ContainerBorder;
            controlPosition.width -= SerializedPropertyLayout.ContainerBorder * 2.0f;
            
            var containerPosition = new Rect(controlPosition.x, controlPosition.yMax + SerializedPropertyLayout.ContainerBorder, controlPosition.width, containerHeight - SerializedPropertyLayout.ContainerBorder * 2.0f);
            EditorGUI.DrawRect(containerPosition, SerializedPropertyLayout.ContainerColor);
            
            controlPosition.x += SerializedPropertyLayout.ContainerPadding;
            controlPosition.y += SerializedPropertyLayout.ContainerPadding;
            controlPosition.width -= SerializedPropertyLayout.ContainerPadding * 2.0f;

            foreach (var serializedProperty in childrenSerializedProperties)
            {
                controlPosition.y = controlPosition.yMax + SerializedPropertyLayout.PropertyPadding;
                controlPosition.height = EditorGUI.GetPropertyHeight(serializedProperty);
                EditorGUI.PropertyField(controlPosition, serializedProperty, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var basePropertyHeight = base.GetPropertyHeight(property, label);
            return SerializedPropertyLayout.GetPropertyHeight(property, label, basePropertyHeight);
        }
    }
}