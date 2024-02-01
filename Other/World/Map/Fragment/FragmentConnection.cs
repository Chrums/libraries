using System;
namespace Fizz6.Roguelike.World.Map.Fragment
{
    [Flags]
    public enum FragmentConnection
    {
        North = 0b0001,
        East  = 0b0010,
        South = 0b0100,
        West  = 0b1000,
    }
}