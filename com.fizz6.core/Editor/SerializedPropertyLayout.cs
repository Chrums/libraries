using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Core.Editor
{
    public class SerializedPropertyLayout
    {
        public const float PropertyPadding = 2.0f;
        
        public static readonly Color BorderColor = new Color32(36, 36, 36, 255);
        public static readonly Color ContainerColor = new Color32(65, 65, 65, 255);
        public const float ContainerBorder = 0.5f;
        public const float ContainerPadding = 4.0f;

        public static float GetPropertyHeight(SerializedProperty property, GUIContent label, float basePropertyHeight)
        {
            // If there's a custom property drawer defined for the selected type, use its height
            // Otherwise calculate it based off the children
            var propertyValueType = property.GetValueType();
            if (propertyValueType != null && propertyValueType.TryGetPropertyDrawer(out var propertyDrawerType))
            {
                var propertyDrawer = (PropertyDrawer)Activator.CreateInstance(propertyDrawerType);
                return basePropertyHeight + 
                       propertyDrawer.GetPropertyHeight(property, label) + 
                       PropertyPadding * 2.0f;
            }

            var containerHeight = GetContainerHeight(property);
            return containerHeight > 0.0f
                ? basePropertyHeight + containerHeight + PropertyPadding * 2.0f
                : basePropertyHeight;
        }
        
        public static float GetContainerHeight(SerializedProperty property)
        {
            var childrenHeight = GetChildrenHeight(property);
            return childrenHeight > 0.0f
                ? childrenHeight + ContainerBorder * 2.0f + ContainerPadding * 2.0f
                : 0.0f;
        }
        
        public static float GetChildrenHeight(SerializedProperty property)
        {
            var childrenSerializedProperties = property
                .GetChildren();
            return childrenSerializedProperties.Any()
                ? childrenSerializedProperties
                    .Aggregate(
                        PropertyPadding, // Above children
                        (current, serializedProperty) => current + EditorGUI.GetPropertyHeight(serializedProperty) + PropertyPadding // Below each child
                    )
                : 0.0f;
        }
    }
}
