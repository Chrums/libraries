using System;
using System.Linq;
using System.Reflection;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableAssembly))]
    public class SerializableAssemblyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var serializableAssembly = property.GetValue<SerializableAssembly>();

            if (serializableAssembly == null)
                return;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var buttonPosition = EditorGUI.PrefixLabel(position, label);
            var content = new GUIContent(serializableAssembly.Value?.GetName().Name);

            if (!GUI.Button(buttonPosition, content, EditorStyles.popup)) 
                return;
            
            var dropdown = 
                new SearchableDropdown<Assembly>(
                    new SearchableDropdown<Assembly>.State(),
                    nameof(Assembly),
                    assemblies
                        .Select(assembly => new SearchableDropdown<Assembly>.Item(assembly, assembly.GetName().Name))
                        .ToArray(),
                    assembly => serializableAssembly.Value = assembly
                );

            dropdown.Show(buttonPosition);
        }
    }
}
