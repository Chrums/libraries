using System.Collections.Generic;
using UnityEditor;

namespace Fizz6.Core.Editor
{
    public class SerializedPropertyEditor
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly Dictionary<string, ReorderableList> _lists = new Dictionary<string, ReorderableList>();
            
        public SerializedPropertyEditor(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty;
            var serializedProperties = _serializedProperty.GetChildren();
                
            foreach (var childSerializedProperty in serializedProperties)
            {
                if (!childSerializedProperty.isArray) continue;
                
                var copy = childSerializedProperty.Copy();
                var reorderableList = new ReorderableList(copy);

                _lists.Add(copy.name, reorderableList);
            }
        }

        public void Render()
        {
            var serializedProperties = _serializedProperty.GetChildren();
            foreach (var serializedProperty in serializedProperties)
            {
                if (serializedProperty.isArray && _lists.TryGetValue(serializedProperty.name, out var list))
                {
                    list.Render();
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedProperty);
                }
            }
        }
    }
}