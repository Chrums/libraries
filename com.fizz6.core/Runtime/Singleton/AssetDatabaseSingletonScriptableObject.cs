using System.Linq;
using UnityEditor;

namespace Fizz6.Core.Editor
{
    public abstract class AssetDatabaseSingletonScriptableObject<T> : SingletonScriptableObject<T> where T : AssetDatabaseSingletonScriptableObject<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var guid = AssetDatabase.FindAssets($"t: {typeof(T).Name}")
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(guid))
                    return null;

                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                    return null;

                _instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                return _instance;
            }
        }
    }
}