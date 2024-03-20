using UnityEditor;
using Object = UnityEngine.Object;

namespace Fizz6.Core.Editor
{
    public class SerializedPropertyWindow : EditorWindow
    {
        public static void For(SerializedProperty serializedProperty, string title = null)
        {
            var window = CreateWindow<SerializedPropertyWindow>(title ?? serializedProperty.displayName);
            window.Initialize(serializedProperty);
            window.Show();
        }

        private SerializedProperty _serializedProperty;
        private SerializedPropertyEditor _serializedPropertyEditor;

        private void Initialize(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty;
            _serializedPropertyEditor = new SerializedPropertyEditor(_serializedProperty);
        }
        
        protected void OnGUI()
        {
            if (_serializedProperty == null) return;

            _serializedProperty.serializedObject.Update();
            
            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(
                        "Target",
                        _serializedProperty.serializedObject.targetObject,
                        typeof(Object),
                        true
                    );
                
                    EditorGUILayout.LabelField(
                        "Property",
                        _serializedProperty.displayName
                    );
                }
            }

            using (new EditorGUILayout.VerticalScope("box"))
            {
                _serializedPropertyEditor.Render();
            }

            _serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}