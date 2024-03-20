using System.Collections.Generic;

namespace Fizz6.Core
{
    public static class ReadOnlyListExt
    {
        public static T Random<T>(this IReadOnlyList<T> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}