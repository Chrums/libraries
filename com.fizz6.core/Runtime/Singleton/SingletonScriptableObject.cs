using UnityEngine;

namespace Fizz6.Core
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {}
}