using System.Collections.Generic;

namespace Fizz6.Core
{
    public static class ListExt
    {
        public static T Random<T>(this List<T> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}