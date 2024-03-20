using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Core.Tests
{
    [InitializeOnLoad]
    public static class AssetModificationProcessorManagerTest
    {
        private const string TestAssetPath = "Assets/Test.cs.meta";
        
        private sealed class AssetModificationProcessorManagerTestDestructor
        {
            ~AssetModificationProcessorManagerTestDestructor()
            {
                AssetModificationProcessorManager.RemoveWillCreateAssetCallback(OnWillCreateAsset);
                AssetModificationProcessorManager.RemoveWillCreateAssetAtPathCallback(TestAssetPath, OnWillCreateAssetAtPath);
                AssetModificationProcessorManager.RemoveWillDeleteAssetCallback(OnWillDeleteAsset);
                AssetModificationProcessorManager.RemoveWillDeleteAssetAtPathCallback(TestAssetPath, OnWillDeleteAssetAtPath);
                AssetModificationProcessorManager.RemoveWillDeleteAssetOfTypeCallback<MonoScript>(OnWillDeleteAssetOfType);
                AssetModificationProcessorManager.RemoveWillMoveAssetCallback(OnWillMoveAsset);
                AssetModificationProcessorManager.RemoveWillMoveAssetOfTypeCallback<MonoScript>(OnWillMoveAssetOfType);
                AssetModificationProcessorManager.RemoveWillSaveAssetCallback(OnWillSaveAsset);
                AssetModificationProcessorManager.RemoveWillSaveAssetAtPathCallback(TestAssetPath, OnWillSaveAssetAtPath);
                AssetModificationProcessorManager.RemoveWillSaveAssetOfTypeCallback<MonoScript>(OnWillSaveAssetOfType);
            }
        }
        
        private static readonly AssetModificationProcessorManagerTestDestructor Destructor = new();
        
        static AssetModificationProcessorManagerTest()
        {
            AssetModificationProcessorManager.AddWillCreateAssetCallback(OnWillCreateAsset);
            AssetModificationProcessorManager.AddWillCreateAssetAtPathCallback(TestAssetPath, OnWillCreateAssetAtPath);
            AssetModificationProcessorManager.AddWillDeleteAssetCallback(OnWillDeleteAsset);
            AssetModificationProcessorManager.AddWillDeleteAssetAtPathCallback(TestAssetPath, OnWillDeleteAssetAtPath);
            AssetModificationProcessorManager.AddWillDeleteAssetOfTypeCallback<MonoScript>(OnWillDeleteAssetOfType);
            AssetModificationProcessorManager.AddWillMoveAssetCallback(OnWillMoveAsset);
            AssetModificationProcessorManager.AddWillMoveAssetOfTypeCallback<MonoScript>(OnWillMoveAssetOfType);
            AssetModificationProcessorManager.AddWillSaveAssetCallback(OnWillSaveAsset);
            AssetModificationProcessorManager.AddWillSaveAssetAtPathCallback(TestAssetPath, OnWillSaveAssetAtPath);
            AssetModificationProcessorManager.AddWillSaveAssetOfTypeCallback<MonoScript>(OnWillSaveAssetOfType);
        }

        private static void OnWillCreateAsset(string assetPath) =>
            Debug.LogError($"{nameof(OnWillCreateAsset)} - {assetPath}");

        private static void OnWillCreateAssetAtPath(string assetPath) =>
            Debug.LogError($"{nameof(OnWillCreateAssetAtPath)} - {assetPath}");

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            Debug.LogError($"{nameof(OnWillDeleteAsset)} - {assetPath}, {options}");
            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetDeleteResult OnWillDeleteAssetAtPath(string assetPath, RemoveAssetOptions options)
        {
            Debug.LogError($"{nameof(OnWillDeleteAssetAtPath)} - {assetPath}, {options}");
            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetDeleteResult OnWillDeleteAssetOfType(string assetPath, RemoveAssetOptions options)
        {
            Debug.LogError($"{nameof(OnWillDeleteAssetOfType)} - {assetPath}, {options}");
            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            Debug.LogError($"{nameof(OnWillMoveAsset)} - {sourcePath}, {destinationPath}");
            return AssetMoveResult.DidNotMove;
        }

        private static AssetMoveResult OnWillMoveAssetOfType(string sourcePath, string destinationPath)
        {
            Debug.LogError($"{nameof(OnWillMoveAssetOfType)} - {sourcePath}, {destinationPath}");
            return AssetMoveResult.DidNotMove;
        }

        private static bool OnWillSaveAsset(string assetPath)
        {
            Debug.LogError($"{nameof(OnWillSaveAsset)} - {assetPath}");
            return true;
        }

        private static bool OnWillSaveAssetAtPath(string assetPath)
        {
            Debug.LogError($"{nameof(OnWillSaveAssetAtPath)} - {assetPath}");
            return true;
        }

        private static bool OnWillSaveAssetOfType(string assetPath)
        {
            Debug.LogError($"{nameof(OnWillSaveAssetOfType)} - {assetPath}");
            return true;
        }
    }
}
