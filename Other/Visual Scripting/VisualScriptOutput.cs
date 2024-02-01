using System;
using Unity.VisualScripting;

namespace Fizz6.VisualScripting
{
    public class VisualScriptOutput : VisualScript
    {
        private readonly VisualScriptOutputUnit visualScriptOutputUnit;

        public event Action<object[]> DataEvent;
        public object[] Current { get; private set; }
        
        public VisualScriptOutput(ScriptMachine scriptMachine) : base(scriptMachine)
        {
            if (!ScriptMachine.graph.TryGetUnit(out visualScriptOutputUnit))
            {
                throw new Exception($"{nameof(VisualScriptOutput)} requires a {nameof(VisualScriptOutputUnit)}");
            }

            visualScriptOutputUnit.DataEvent += OnData;
        }

        ~VisualScriptOutput()
        {
            visualScriptOutputUnit.DataEvent -= OnData;
        }

        private void OnData(object[] data)
        {
            Current = data;
            DataEvent?.Invoke(data);
        }
    }
}