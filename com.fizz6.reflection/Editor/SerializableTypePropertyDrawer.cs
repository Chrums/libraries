using System;
using System.Linq;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var serializableType = property.GetValue<SerializableType>();

            if (serializableType == null)
                return;

            var types = ReflectionConfig.Instance.Types;

            var buttonPosition = EditorGUI.PrefixLabel(position, label);
            var content = new GUIContent(serializableType.Value?.Name);

            if (!GUI.Button(buttonPosition, content, EditorStyles.popup)) 
                return;
            
            var dropdown =
                new SearchableDropdown<Type>(
                    new SearchableDropdown<Type>.State(),
                    nameof(Type),
                    types
                        .Select(type => new SearchableDropdown<Type>.Item(type, $"{type.Assembly.GetName().Name}/{type.Name}"))
                        .ToArray(),
                    type => serializableType.Value = type
                );
            
            dropdown.Show(buttonPosition);
        }
    }
}
