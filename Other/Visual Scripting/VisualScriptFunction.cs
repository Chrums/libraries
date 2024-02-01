using Unity.VisualScripting;

namespace Fizz6.VisualScripting
{
    public class VisualScriptFunction : VisualScript
    {
        protected VisualScriptInput VisualScriptInput { get; }
        protected VisualScriptOutput VisualScriptOutput { get; }

        public VisualScriptFunction(ScriptMachine scriptMachine) : base(scriptMachine)
        {
            VisualScriptInput = new VisualScriptInput(scriptMachine);
            VisualScriptOutput = new VisualScriptOutput(scriptMachine);
        }

        public object[] Invoke(params object[] arguments)
        {
            VisualScriptInput.Invoke(arguments);
            return VisualScriptOutput.Current;
        }
    }
}