using UnityEditor;

namespace Fizz6.Data.Editor
{
    [CustomEditor(typeof(ModelConfig))]
    public class ModelConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true)) 
                base.OnInspectorGUI();
        }
    }
}
