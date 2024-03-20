using System;
using System.Collections;
using UnityEditor;

namespace Fizz6.Core.Editor
{
    public class ReorderableList
    {
        private readonly SerializedProperty _serializedProperty;
        private readonly UnityEditorInternal.ReorderableList _reorderableList;

        private bool _isExpanded = true;
        
        public ReorderableList(SerializedProperty serializedProperty)
        {
            if (!serializedProperty.isArray) return;

            _serializedProperty = serializedProperty;
            
            var copy = serializedProperty.Copy();
            _reorderableList =
                new UnityEditorInternal.ReorderableList(copy.serializedObject, copy, true, true, true, true)
                {
                    headerHeight = 0
                };

            _reorderableList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    var arrayElementSerializedProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, arrayElementSerializedProperty); 
                };
            
            _reorderableList.onAddCallback =
                _ =>
                {
                    copy.arraySize++;
                    var index = copy.arraySize - 1;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                    
                    var arrayElementSerializedProperty = copy.GetArrayElementAtIndex(index);
                    if (arrayElementSerializedProperty.propertyType != SerializedPropertyType.Generic) return;
                    var childType = arrayElementSerializedProperty
                        .GetValue()
                        .GetType();
                    
                    if (copy.GetValue() is IList list) list[index] = Activator.CreateInstance(childType);
                };
        }

        public void Render()
        {
            _isExpanded = EditorGUILayout.Foldout(_isExpanded, ObjectNames.NicifyVariableName(_serializedProperty.name));
            if (!_isExpanded) return;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space(16.0f, false);
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.Space(2.0f, false);
                    _serializedProperty.arraySize = EditorGUILayout.IntField("Size", _serializedProperty.arraySize);
                    EditorGUILayout.Space(2.0f, false);
                    _reorderableList.DoLayoutList();
                    EditorGUILayout.Space(2.0f, false);
                }
            }
        }
    }
}
