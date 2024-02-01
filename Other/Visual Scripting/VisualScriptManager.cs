using Unity.VisualScripting;
using UnityEngine;

namespace Fizz6.VisualScripting
{
    public class VisualScriptManager : SingletonMonoBehaviour<VisualScriptManager>
    {
        private const string ScriptMachinesGameObjectName = "[ScriptMachines]";
        private GameObject scriptMachinesGameObject;

        private void Awake()
        {
            scriptMachinesGameObject = new GameObject
            {
                name = ScriptMachinesGameObjectName,
                transform =
                {
                    parent = transform
                }
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(scriptMachinesGameObject);
        }
        
        public ScriptMachine Instantiate(ScriptGraphAsset scriptGraphAsset)
        {
            var scriptMachine = scriptMachinesGameObject.AddComponent<ScriptMachine>();
            scriptMachine.nest.source = GraphSource.Macro;
            scriptMachine.nest.macro = scriptGraphAsset;
            return scriptMachine;
        }
    }
}