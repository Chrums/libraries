using UnityEngine;

namespace Fizz6.Core
{
    public abstract class ResourceSingletonScriptableObject<T> : SingletonScriptableObject<T> where T : ResourceSingletonScriptableObject<T>
    {
        private static T _instance;
        public static T Instance => _instance ??= Resources.Load<T>(typeof(T).Name);
    }
}