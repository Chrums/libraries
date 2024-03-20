using UnityEngine.AddressableAssets;

namespace Fizz6.Core
{
    public abstract class AddressableSingletonScriptableObject<T> : SingletonScriptableObject<T> where T : AddressableSingletonScriptableObject<T>
    {
        private static T _instance;
        public static T Instance => _instance ??= Addressables.LoadAssetAsync<T>(typeof(T).Name).WaitForCompletion();
    }
}