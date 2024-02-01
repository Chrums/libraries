using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Fizz6.VisualScripting
{
    public class VisualScriptInputUnit : Unit
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
        private ControlOutput controlOutput;
        
        [DoNotSerialize]
        private readonly List<ValueOutput> argumentOutputs = new();
        
        public void Invoke(GraphReference graphReference, object[] arguments)
        {
            var flow = Flow.New(graphReference);
            
            if (arguments.Length != ArgumentCount)
            {
                Debug.LogError($"'{nameof(ArgumentCount)}' must equal the number of arguments passed to '{nameof(Invoke)}'" +
                               $"\n{nameof(ArgumentCount)}: {ArgumentCount}" +
                               $"\n{nameof(Invoke)}: {arguments.Length}");
                return;
            }
            
            for (var index = 0; index < ArgumentCount; index++)
            {
                var output = argumentOutputs[index];
                var argument = arguments[index];
                flow.SetValue(output, argument);
            }
            
            flow.Run(controlOutput);
        }
        
        protected override void Definition()
        {
            controlOutput = ControlOutput(nameof(controlOutput));
            
            argumentOutputs.Clear();
            for (var index = 0; index < ArgumentCount; index++)
            {
                var key = $"Argument {index}";
                var argumentOutput = ValueOutput<object>(key);
                argumentOutputs.Add(argumentOutput);
            }
        }
    }
}