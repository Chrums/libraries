using Unity.VisualScripting;

namespace Fizz6.VisualScripting
{
    public class VisualScript
    {
        protected ScriptMachine ScriptMachine { get; }
        
        public VisualScript(ScriptMachine scriptMachine)
        {
            ScriptMachine = scriptMachine;
        }
    }
}