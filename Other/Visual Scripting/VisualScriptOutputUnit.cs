using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Fizz6.VisualScripting
{
    public class VisualScriptOutputUnit : Unit
    {
        [Serialize]
        private int argumentCount;
        
        [DoNotSerialize, Inspectable, UnitHeaderInspectable("Arguments")]
        public int ArgumentCount
        {
            get => argumentCount;
            set => argumentCount = Math.Clamp(value, 0, 10);
        }
        
        [DoNotSerialize, PortLabelHidden]
        private ControlInput controlInput;

        [DoNotSerialize, PortLabelHidden]
        private ControlOutput controlOutput;
        
        [DoNotSerialize]
        private readonly List<ValueInput> argumentInputs = new();

        public event Action<object[]> DataEvent;
        public object[] Current { get; private set; }
        
        protected override void Definition()
        {
            controlInput = ControlInput(nameof(controlInput), Trigger);
            controlOutput = ControlOutput(nameof(controlOutput));
            Succession(controlInput, controlOutput);
            
            argumentInputs.Clear();
            for (var index = 0; index < ArgumentCount; index++)
            {
                var key = $"Argument {index}";
                var argumentInput = ValueInput<object>(key);
                argumentInputs.Add(argumentInput);
            }
        }

        private ControlOutput Trigger(Flow flow)
        {
            var arguments = argumentInputs
                .Select(flow.GetValue<object>)
                .ToArray();
            
            Current = arguments;
            DataEvent?.Invoke(Current);
            
            return controlOutput;
        }
    }
}