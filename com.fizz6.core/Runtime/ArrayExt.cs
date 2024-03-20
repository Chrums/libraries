using System;

namespace Fizz6.Core
{
    public static class ArrayExt
    {
        public static void ForEach<T>(T[,] array, Action<int, int> callback)
        {
            for (var x = 0; x < array.GetLength(0); ++x)
            {
                for (var y = 0; y < array.GetLength(1); ++y)
                {
                    callback?.Invoke(x, y);
                }
            }
        }
        
        public static void ForEach<T>(T[,,] array, Action<int, int, int> callback)
        {
            for (var x = 0; x < array.GetLength(0); ++x)
            {
                for (var y = 0; y < array.GetLength(1); ++y)
                {
                    for (var z = 0; z < array.GetLength(2); ++z)
                    {
                        callback?.Invoke(x, y, z);
                    }
                }
            }
        }
    }
}