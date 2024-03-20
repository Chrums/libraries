using System;
using System.Collections.Generic;
using UnityEditor;

namespace Fizz6.Core.Editor
{
    public class AssetPostprocessorManager : AssetPostprocessor
    {
        public delegate void BeforePostprocessAllAssetsCallback();
        public static event BeforePostprocessAllAssetsCallback BeforePostprocessAllAssetsEvent;
        
        public delegate void AfterPostprocessAllAssetsCallback();
        private static event AfterPostprocessAllAssetsCallback AfterPostprocessAllAssetsEvent;

        public delegate void ImportedAssetsCallback(IReadOnlyList<string> assetPaths);
        public static event ImportedAssetsCallback ImportedAssetsEvent;
        
        public delegate void ImportedAssetCallback(string assetPath);
        public static event ImportedAssetCallback ImportedAssetEvent;

        public delegate void DeletedAssetsCallback(IReadOnlyList<string> assetPaths);
        public static event DeletedAssetsCallback DeletedAssetsEvent;

        public delegate void DeletedAssetCallback(string assetPath);
        public static event DeletedAssetCallback DeletedAssetEvent;

        public delegate void MovedAssetsCallback(IReadOnlyList<(string, string)> assetPaths);
        public static event MovedAssetsCallback MovedAssetsEvent;

        public delegate void MovedAssetCallback(string sourcePath, string destinationPath);
        private static event MovedAssetCallback MovedAssetEvent;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            BeforePostprocessAllAssetsEvent?.Invoke();
            
            ImportedAssetsEvent?.Invoke(importedAssets);
            foreach (var importedAsset in importedAssets)
                ImportedAssetEvent?.Invoke(importedAsset);

            DeletedAssetsEvent?.Invoke(deletedAssets);
            foreach (var deletedAsset in deletedAssets)
                DeletedAssetEvent?.Invoke(deletedAsset);

            var movedAssetTuples = new List<(string, string)>();
            for (var index = 0; index < Math.Min(movedAssets.Length, movedFromAssetPaths.Length); ++index)
            {
                var sourcePath = movedFromAssetPaths[index];
                var destinationPath = movedAssets[index];
                movedAssetTuples.Add((sourcePath, destinationPath));
            }

            MovedAssetsEvent?.Invoke(movedAssetTuples);
            foreach (var movedAssetTuple in movedAssetTuples)
                MovedAssetEvent?.Invoke(movedAssetTuple.Item2, movedAssetTuple.Item2);
            
            AfterPostprocessAllAssetsEvent?.Invoke();
        }
    }
}
