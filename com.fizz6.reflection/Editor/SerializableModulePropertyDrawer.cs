using System;
using System.Linq;
using System.Reflection;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CustomPropertyDrawer(typeof(SerializableModule))]
    public class SerializableModulePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var serializableModule = property.GetValue<SerializableModule>();

            if (serializableModule == null)
                return;

            var modules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.Modules)
                .ToArray();

            var buttonPosition = EditorGUI.PrefixLabel(position, label);
            var content = new GUIContent(serializableModule.Value?.Name);

            if (!GUI.Button(buttonPosition, content, EditorStyles.popup)) 
                return;
            
            var dropdown =
                new SearchableDropdown<Module>(
                    new SearchableDropdown<Module>.State(),
                    nameof(Module),
                    modules
                        .Select(module => new SearchableDropdown<Module>.Item(module, $"{module.Assembly.GetName().Name}/{module.Name}"))
                        .ToArray(),
                    module => serializableModule.Value = module
                );

            dropdown.Show(buttonPosition);
        }
    }
}
