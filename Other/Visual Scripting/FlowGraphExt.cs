using System.Linq;
using Unity.VisualScripting;

namespace Fizz6.VisualScripting
{
    public static class FlowGraphExt
    {
        public static TUnit GetUnit<TUnit>(this FlowGraph flowGraph) where TUnit : Unit => 
            flowGraph.units
                .FirstOrDefault(unit => unit.GetType() == typeof(TUnit)) as TUnit;

        public static bool TryGetUnit<TUnit>(this FlowGraph flowGraph, out TUnit unit) where TUnit : Unit
        {
            unit = flowGraph.GetUnit<TUnit>();
            return unit != null;
        }

        public static TUnit[] GetUnits<TUnit>(this FlowGraph flowGraph) where TUnit : Unit =>
            flowGraph.units
                .Where(unit => unit.GetType() == typeof(TUnit))
                .Select(unit => unit as TUnit)
                .ToArray();

        public static bool TryGetUnits<TUnit>(this FlowGraph flowGraph, out TUnit[] units) where TUnit : Unit
        {
            units = flowGraph.GetUnits<TUnit>();
            return units.Length > 0;
        }
    }
}
