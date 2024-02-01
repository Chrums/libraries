using System;
using Unity.VisualScripting;

namespace Fizz6.VisualScripting
{
    public class VisualScriptInput : VisualScript
    {
        private readonly VisualScriptInputUnit visualScriptInputUnit;
        
        public VisualScriptInput(ScriptMachine scriptMachine) : base(scriptMachine)
        {
            if (!ScriptMachine.graph.TryGetUnit(out visualScriptInputUnit))
            {
                throw new Exception($"{nameof(VisualScriptInput)} requires a {nameof(VisualScriptInputUnit)}");
            }
        }

        public void Invoke(params object[] arguments)
        {
            if (ScriptMachine.nest.source == GraphSource.Embed)
            {
                throw new Exception($"{nameof(VisualScriptInput)} requires {nameof(ScriptMachine.nest.source)} to be {nameof(GraphSource.Macro)}");
            }
            
            var reference = ScriptMachine.nest.macro
                .GetReference()
                .AsReference();
            visualScriptInputUnit.Invoke(reference, arguments);
        }
    }
}