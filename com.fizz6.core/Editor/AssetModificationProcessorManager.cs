using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Core.Editor
{
    public class AssetModificationProcessorManager : AssetModificationProcessor
    {
        public delegate void WillCreateAssetCallback(string assetPath);
        private static readonly List<WillCreateAssetCallback> WillCreateAssetCallbacks = new();
        private static readonly Dictionary<string, List<WillCreateAssetCallback>> WillCreateAssetAtPathCallbacks = new();

        public delegate AssetDeleteResult WillDeleteAssetCallback(string assetPath, RemoveAssetOptions options);
        private static readonly List<WillDeleteAssetCallback> WillDeleteAssetCallbacks = new();
        private static readonly Dictionary<string, List<WillDeleteAssetCallback>> WillDeleteAssetAtPathCallbacks = new();
        private static readonly Dictionary<Type, List<WillDeleteAssetCallback>> WillDeleteAssetOfTypeCallbacks = new();

        public delegate AssetMoveResult WillMoveAssetCallback(string sourcePath, string destinationPath);
        private static readonly List<WillMoveAssetCallback> WillMoveAssetCallbacks = new();
        private static readonly Dictionary<string, List<WillMoveAssetCallback>> WillMoveAssetFromPathCallbacks = new();
        private static readonly Dictionary<string, List<WillMoveAssetCallback>> WillMoveAssetToPathCallbacks = new();
        private static readonly Dictionary<Type, List<WillMoveAssetCallback>> WillMoveAssetOfTypeCallbacks = new();

        public delegate bool WillSaveAssetCallback(string assetPath);
        private static readonly List<WillSaveAssetCallback> WillSaveAssetCallbacks = new();
        private static readonly Dictionary<string, List<WillSaveAssetCallback>> WillSaveAssetAtPathCallbacks = new();
        private static readonly Dictionary<Type, List<WillSaveAssetCallback>> WillSaveAssetOfTypeCallbacks = new();

        #region WillCreateAsset

        public static void AddWillCreateAssetCallback(WillCreateAssetCallback willCreateAssetCallback) =>
            WillCreateAssetCallbacks.Add(willCreateAssetCallback);

        public static void RemoveWillCreateAssetCallback(WillCreateAssetCallback willCreateAssetCallback) =>
            WillCreateAssetCallbacks.Remove(willCreateAssetCallback);

        public static void AddWillCreateAssetAtPathCallback(string assetPath, WillCreateAssetCallback willCreateAssetCallback)
        {
            if (!WillCreateAssetAtPathCallbacks.TryGetValue(assetPath, out var willCreateAssetWithNameCallbacks))
            {
                willCreateAssetWithNameCallbacks = new List<WillCreateAssetCallback>();
                WillCreateAssetAtPathCallbacks[assetPath] = willCreateAssetWithNameCallbacks;
            }
            
            willCreateAssetWithNameCallbacks.Add(willCreateAssetCallback);
        }

        public static void RemoveWillCreateAssetAtPathCallback(string assetPath, WillCreateAssetCallback willCreateAssetCallback)
        {
            if (!WillCreateAssetAtPathCallbacks.TryGetValue(assetPath, out var willCreateAssetWithNameCallbacks))
            {
                willCreateAssetWithNameCallbacks = new List<WillCreateAssetCallback>();
                WillCreateAssetAtPathCallbacks[assetPath] = willCreateAssetWithNameCallbacks;
            }
            
            willCreateAssetWithNameCallbacks.Remove(willCreateAssetCallback);
        }

        private static void OnWillCreateAsset(string assetPath)
        {
            foreach (var willCreateAssetCallback in WillCreateAssetCallbacks)
                willCreateAssetCallback.Invoke(assetPath);

            if (!WillCreateAssetAtPathCallbacks.TryGetValue(assetPath, out var willCreateAssetAtPathCallbacks))
                return;
            
            foreach (var willCreateAssetAtPathCallback in willCreateAssetAtPathCallbacks)
                willCreateAssetAtPathCallback.Invoke(assetPath);
        }
        
        #endregion
        
        #region WillDeleteAsset

        public static void AddWillDeleteAssetCallback(WillDeleteAssetCallback willDeleteAssetCallback) =>
            WillDeleteAssetCallbacks.Add(willDeleteAssetCallback);

        public static void RemoveWillDeleteAssetCallback(WillDeleteAssetCallback willDeleteAssetCallback) =>
            WillDeleteAssetCallbacks.Remove(willDeleteAssetCallback);

        public static void AddWillDeleteAssetAtPathCallback(string assetPath, WillDeleteAssetCallback willDeleteAssetCallback)
        {
            if (!WillDeleteAssetAtPathCallbacks.TryGetValue(assetPath, out var willDeleteAssetAtPathCallbacks))
            {
                willDeleteAssetAtPathCallbacks = new List<WillDeleteAssetCallback>();
                WillDeleteAssetAtPathCallbacks[assetPath] = willDeleteAssetAtPathCallbacks;
            }
            
            willDeleteAssetAtPathCallbacks.Add(willDeleteAssetCallback);
        }

        public static void RemoveWillDeleteAssetAtPathCallback(string assetPath, WillDeleteAssetCallback willDeleteAssetCallback)
        {
            if (!WillDeleteAssetAtPathCallbacks.TryGetValue(assetPath, out var willDeleteAssetAtPathCallbacks))
            {
                willDeleteAssetAtPathCallbacks = new List<WillDeleteAssetCallback>();
                WillDeleteAssetAtPathCallbacks[assetPath] = willDeleteAssetAtPathCallbacks;
            }
            
            willDeleteAssetAtPathCallbacks.Remove(willDeleteAssetCallback);
        }

        public static void AddWillDeleteAssetOfTypeCallback<T>(WillDeleteAssetCallback willDeleteAssetCallback)
        {
            var type = typeof(T);
            if (!WillDeleteAssetOfTypeCallbacks.TryGetValue(type, out var willDeleteAssetOfTypeCallbacks))
            {
                willDeleteAssetOfTypeCallbacks = new List<WillDeleteAssetCallback>();
                WillDeleteAssetOfTypeCallbacks[type] = willDeleteAssetOfTypeCallbacks;
            }
            
            willDeleteAssetOfTypeCallbacks.Add(willDeleteAssetCallback);
        }

        public static void RemoveWillDeleteAssetOfTypeCallback<T>(WillDeleteAssetCallback willDeleteAssetCallback)
        {
            var type = typeof(T);
            if (!WillDeleteAssetOfTypeCallbacks.TryGetValue(type, out var willDeleteAssetOfTypeCallbacks))
            {
                willDeleteAssetOfTypeCallbacks = new List<WillDeleteAssetCallback>();
                WillDeleteAssetOfTypeCallbacks[type] = willDeleteAssetOfTypeCallbacks;
            }
            
            willDeleteAssetOfTypeCallbacks.Remove(willDeleteAssetCallback);
        }

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var aggregateAssetDeleteResult = AssetDeleteResult.DidNotDelete;

            AssetDeleteResult Aggregate(AssetDeleteResult assetDeleteResult0, AssetDeleteResult assetDeleteResult1)
            {
                if (assetDeleteResult0 is AssetDeleteResult.DidNotDelete || assetDeleteResult1 is AssetDeleteResult.DidDelete)
                    return (AssetDeleteResult)Math.Max((int)assetDeleteResult0, (int)assetDeleteResult1);

                if (assetDeleteResult0 is AssetDeleteResult.FailedDelete && assetDeleteResult1 is AssetDeleteResult.FailedDelete)
                    return AssetDeleteResult.FailedDelete;
                
                Debug.LogError($"{nameof(AssetDeleteResult.DidDelete)} or {nameof(AssetDeleteResult.FailedDelete)} returned by multiple callbacks");
                return AssetDeleteResult.DidDelete;
            }

            foreach (var willDeleteAssetCallback in WillDeleteAssetCallbacks)
            {
                var assetDeleteResult = willDeleteAssetCallback.Invoke(assetPath, options);
                aggregateAssetDeleteResult = Aggregate(aggregateAssetDeleteResult, assetDeleteResult);
            }

            if (WillDeleteAssetAtPathCallbacks.TryGetValue(assetPath, out var willDeleteAssetAtPathCallbacks))
            {
                foreach (var willDeleteAssetAtPathCallback in willDeleteAssetAtPathCallbacks)
                {
                    var assetDeleteResult = willDeleteAssetAtPathCallback.Invoke(assetPath, options);
                    aggregateAssetDeleteResult = Aggregate(aggregateAssetDeleteResult, assetDeleteResult);
                }
            }
            
            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (type != null && WillDeleteAssetOfTypeCallbacks.TryGetValue(type, out var willDeleteAssetOfTypeCallbacks))
            {
                foreach (var willDeleteAssetOfTypeCallback in willDeleteAssetOfTypeCallbacks)
                {
                    var assetDeleteResult = willDeleteAssetOfTypeCallback.Invoke(assetPath, options);
                    aggregateAssetDeleteResult = Aggregate(aggregateAssetDeleteResult, assetDeleteResult);
                }
            }

            return aggregateAssetDeleteResult;
        }
        
        #endregion
        
        #region WillMoveAsset
        
        public static void AddWillMoveAssetCallback(WillMoveAssetCallback willMoveAssetCallback) =>
            WillMoveAssetCallbacks.Add(willMoveAssetCallback);

        public static void RemoveWillMoveAssetCallback(WillMoveAssetCallback willMoveAssetCallback) =>
            WillMoveAssetCallbacks.Remove(willMoveAssetCallback);

        public static void AddWillMoveAssetOfTypeCallback<T>(WillMoveAssetCallback willMoveAssetCallback)
        {
            var type = typeof(T);
            if (!WillMoveAssetOfTypeCallbacks.TryGetValue(type, out var willMoveAssetOfTypeCallbacks))
            {
                willMoveAssetOfTypeCallbacks = new List<WillMoveAssetCallback>();
                WillMoveAssetOfTypeCallbacks[type] = willMoveAssetOfTypeCallbacks;
            }
            
            willMoveAssetOfTypeCallbacks.Add(willMoveAssetCallback);
        }

        public static void RemoveWillMoveAssetOfTypeCallback<T>(WillMoveAssetCallback willMoveAssetCallback)
        {
            var type = typeof(T);
            if (!WillMoveAssetOfTypeCallbacks.TryGetValue(type, out var willMoveAssetOfTypeCallbacks))
            {
                willMoveAssetOfTypeCallbacks = new List<WillMoveAssetCallback>();
                WillMoveAssetOfTypeCallbacks[type] = willMoveAssetOfTypeCallbacks;
            }
            
            willMoveAssetOfTypeCallbacks.Remove(willMoveAssetCallback);
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            var aggregateAssetMoveResult = AssetMoveResult.DidNotMove;
            
            AssetMoveResult Aggregate(AssetMoveResult assetMoveResult0, AssetMoveResult assetMoveResult1)
            {
                if (assetMoveResult0 is AssetMoveResult.DidNotMove || assetMoveResult1 is AssetMoveResult.DidNotMove)
                    return (AssetMoveResult)Math.Max((int)assetMoveResult0, (int)assetMoveResult1);

                if (assetMoveResult0 is AssetMoveResult.FailedMove && assetMoveResult1 is AssetMoveResult.FailedMove)
                    return AssetMoveResult.FailedMove;
                
                Debug.LogError($"{nameof(AssetMoveResult.DidMove)} or {nameof(AssetMoveResult.FailedMove)} returned by multiple callbacks");
                return AssetMoveResult.DidMove;
            }
            
            foreach (var willMoveAssetCallback in WillMoveAssetCallbacks)
            {
                var assetMoveResult = willMoveAssetCallback.Invoke(sourcePath, destinationPath);
                aggregateAssetMoveResult = Aggregate(aggregateAssetMoveResult, assetMoveResult);
            }
            
            var type = AssetDatabase.GetMainAssetTypeAtPath(sourcePath);
            if (type != null && WillMoveAssetOfTypeCallbacks.TryGetValue(type, out var willMoveAssetOfTypeCallbacks))
            {
                foreach (var willMoveAssetOfTypeCallback in willMoveAssetOfTypeCallbacks)
                {
                    var assetMoveResult = willMoveAssetOfTypeCallback.Invoke(sourcePath, destinationPath);
                    aggregateAssetMoveResult = Aggregate(aggregateAssetMoveResult, assetMoveResult);
                }
            }

            return aggregateAssetMoveResult;
        }
        
        #endregion

        #region WillSaveAssets

        public static void AddWillSaveAssetCallback(WillSaveAssetCallback willSaveAssetCallback) =>
            WillSaveAssetCallbacks.Add(willSaveAssetCallback);

        public static void RemoveWillSaveAssetCallback(WillSaveAssetCallback willSaveAssetCallback) =>
            WillSaveAssetCallbacks.Remove(willSaveAssetCallback);

        public static void AddWillSaveAssetAtPathCallback(string assetPath, WillSaveAssetCallback willSaveAssetCallback)
        {
            if (!WillSaveAssetAtPathCallbacks.TryGetValue(assetPath, out var willSaveAssetAtPathCallbacks))
            {
                willSaveAssetAtPathCallbacks = new List<WillSaveAssetCallback>();
                WillSaveAssetAtPathCallbacks[assetPath] = willSaveAssetAtPathCallbacks;
            }
            
            willSaveAssetAtPathCallbacks.Add(willSaveAssetCallback);
        }

        public static void RemoveWillSaveAssetAtPathCallback(string assetPath, WillSaveAssetCallback willSaveAssetCallback)
        {
            if (!WillSaveAssetAtPathCallbacks.TryGetValue(assetPath, out var willSaveAssetAtPathCallbacks))
            {
                willSaveAssetAtPathCallbacks = new List<WillSaveAssetCallback>();
                WillSaveAssetAtPathCallbacks[assetPath] = willSaveAssetAtPathCallbacks;
            }
            
            willSaveAssetAtPathCallbacks.Remove(willSaveAssetCallback);
        }

        public static void AddWillSaveAssetOfTypeCallback<T>(WillSaveAssetCallback willSaveAssetCallback)
        {
            var type = typeof(T);
            if (!WillSaveAssetOfTypeCallbacks.TryGetValue(type, out var willSaveAssetOfTypeCallbacks))
            {
                willSaveAssetOfTypeCallbacks = new List<WillSaveAssetCallback>();
                WillSaveAssetOfTypeCallbacks[type] = willSaveAssetOfTypeCallbacks;
            }
            
            willSaveAssetOfTypeCallbacks.Add(willSaveAssetCallback);
        }

        public static void RemoveWillSaveAssetOfTypeCallback<T>(WillSaveAssetCallback willSaveAssetCallback)
        {
            var type = typeof(T);
            if (!WillSaveAssetOfTypeCallbacks.TryGetValue(type, out var willSaveAssetOfTypeCallbacks))
            {
                willSaveAssetOfTypeCallbacks = new List<WillSaveAssetCallback>();
                WillSaveAssetOfTypeCallbacks[type] = willSaveAssetOfTypeCallbacks;
            }
            
            willSaveAssetOfTypeCallbacks.Remove(willSaveAssetCallback);
        }
        
        private static string[] OnWillSaveAssets(string[] paths)
        {
            var aggregatePaths = new HashSet<string>(paths);

            foreach (var path in paths)
            {
                foreach (var willSaveAssetCallback in WillSaveAssetCallbacks)
                {
                    var shouldSave = willSaveAssetCallback.Invoke(path);
                    if (!shouldSave)
                        aggregatePaths.Remove(path);
                }

                if (WillSaveAssetAtPathCallbacks.TryGetValue(path, out var willSaveAssetAtPathCallbacks))
                {
                    foreach (var willSaveAssetAtPathCallback in willSaveAssetAtPathCallbacks)
                    {
                        var shouldSave = willSaveAssetAtPathCallback.Invoke(path);
                        if (!shouldSave)
                            aggregatePaths.Remove(path);
                    }
                }
            
                var type = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (type != null && WillSaveAssetOfTypeCallbacks.TryGetValue(type, out var willSaveAssetOfTypeCallbacks))
                {
                    foreach (var willSaveAssetOfTypeCallback in willSaveAssetOfTypeCallbacks)
                    {
                        var shouldSave = willSaveAssetOfTypeCallback.Invoke(path);
                        if (!shouldSave)
                            aggregatePaths.Remove(path);
                    }
                }
            }

            return aggregatePaths.ToArray();
        }
        
        #endregion
    }
}