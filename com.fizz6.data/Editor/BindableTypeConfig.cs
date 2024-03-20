using System;
using System.Collections.Generic;
using System.Linq;
using Fizz6.Core;
using Fizz6.Core.Editor;
using Fizz6.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Fizz6.Data.Editor
{
    [CreateAssetMenu(fileName = "BindableTypeConfig", menuName = "Fizz6/Data/Bindable Type Config")]
    public class BindableTypeConfig : AssetDatabaseSingletonScriptableObject<BindableTypeConfig>
    {
        private static string AssetsDirectoryPath => "Assets";
        private static string EditorDirectoryName => "Editor";
        private static string EditorDirectoryPath => $"Assets/{EditorDirectoryName}";
        private static string ModelConfigsDirectoryName => $"{nameof(ModelConfig)}s";
        private static string ModelConfigsDirectoryPath => $"{EditorDirectoryPath}/{ModelConfigsDirectoryName}";

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            foreach (var assembly in Instance.assemblies)
                AssemblyTypeModificationProcessor.AddTypesModifiedCallback(assembly, OnTypesModified);
        }

        private static void OnTypesModified(IReadOnlyList<Type> types)
        {
            foreach (var type in types)
                TryRebuild(type, out _);
        }

        [MenuItem("Fizz6/Data/Rebuild BindableTypeConfig")]
        public static void Rebuild()
        {
            var compiledAssemblies = CompilationPipeline.GetAssemblies()
                .Where(
                    compiledAssembly =>
                    {
                        var formattedCompiledAssemblyOutputPath = compiledAssembly.outputPath.Replace("/", "\\");
                        return Instance.assemblies.Any(assembly => assembly.Value != null && assembly.Value.Location.EndsWith(formattedCompiledAssemblyOutputPath));
                    }
                )
                .ToArray();
            
            if (compiledAssemblies.Length == 0)
                return;

            var assetPaths = compiledAssemblies
                .SelectMany(compiledAssembly => compiledAssembly.sourceFiles);
            foreach (var assetPath in assetPaths)
                TryRebuild(assetPath, out _);
        }

        private static bool TryRebuild(string assetPath, out ModelConfig modelConfig)
        {
            modelConfig = null;
            
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (assetType != typeof(MonoScript))
                return false;

            var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (!TypeExt.TryGetTypeByName(monoScript.name, out var type))
                return false;

            return TryRebuild(type, out modelConfig);
        }

        private static bool TryRebuild(Type type, out ModelConfig modelConfig)
        {
            modelConfig = AssetDatabase.FindAssets($"t: {nameof(ModelConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ModelConfig>)
                .FirstOrDefault(modelConfig => modelConfig.Type == type);
            
            var modelType = typeof(IModel);
            if (!modelType.IsAssignableFrom(type))
                return false;

            AssetDatabase.StartAssetEditing();

            try
            {
                if (!AssetDatabase.IsValidFolder(EditorDirectoryPath))
                    AssetDatabase.CreateFolder(AssetsDirectoryPath, EditorDirectoryName);

                if (!AssetDatabase.IsValidFolder(ModelConfigsDirectoryPath))
                    AssetDatabase.CreateFolder(EditorDirectoryPath, ModelConfigsDirectoryName);

                if (modelConfig == null)
                {
                    modelConfig = CreateInstance<ModelConfig>();
                    modelConfig.Type = type;
                    var path = $"{ModelConfigsDirectoryPath}/{type.Name}Config.asset";
                    AssetDatabase.CreateAsset(modelConfig, path);
                }

                if (!modelConfig.TryBuild())
                {
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.Refresh();
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
                return false;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
            
            Debug.Log($"Rebuilt {nameof(ModelConfig)} for {type.Name}");

            var subclassTypes = type.GetSubclassTypes();
            return subclassTypes
                .Aggregate(true, (current, subclassType) => current && TryRebuild(subclassType, out _));
        }
        
        [SerializeField] 
        private SerializableAssembly[] assemblies;

        private void OnDestroy()
        {
            foreach (var assembly in Instance.assemblies)
                AssemblyTypeModificationProcessor.RemoveTypesModifiedCallback(assembly, OnTypesModified);
        }
    }
}
