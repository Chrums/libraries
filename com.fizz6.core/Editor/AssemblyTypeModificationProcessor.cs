using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Fizz6.Core
{
    public static class AssemblyTypeModificationProcessor
    {
        [Serializable]
        private class PersistentAssetPaths
        {
            [Serializable]
            public class Assembly
            {
                [SerializeField]
                private string assemblyLocation;
                public string AssemblyLocation => assemblyLocation;
            
                [SerializeField] 
                private string[] assetPaths;
                public string[] AssetPaths => assetPaths;
                
                public Assembly(string assemblyLocation, string[] assetPaths)
                {
                    this.assemblyLocation = assemblyLocation;
                    this.assetPaths = assetPaths;
                }
            }

            [SerializeField] 
            private List<Assembly> assemblies = new();
            public List<Assembly> Assemblies => assemblies;
        }
        
        private static string EditorDirectoryName => "Editor";
        private static string PersistentAssetPathsFilePath => $"{Application.dataPath}/{EditorDirectoryName}/{nameof(AssemblyTypeModificationProcessor)}.AssetPaths.json";
        
        private static IEnumerable<string> _importedAssetPaths;
        
        public delegate void TypesModifiedCallback(IReadOnlyList<Type> types);
        private static readonly Dictionary<string, List<TypesModifiedCallback>> TypesModifiedCallbacksByAssemblyLocation = new();

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            AssetPostprocessorManager.ImportedAssetsEvent += OnImportedAssets;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        private static void OnImportedAssets(IReadOnlyList<string> assetPaths)
        {
            if (File.Exists(PersistentAssetPathsFilePath))
                File.Delete(PersistentAssetPathsFilePath);
            _importedAssetPaths = assetPaths;
        }

        private static void OnAssemblyCompilationFinished(string assemblyOutputPath, CompilerMessage[] compilerMessages)
        {
            var importedAssetPaths = _importedAssetPaths;
            _importedAssetPaths = null;

            if (importedAssetPaths == null)
                return;

            var formattedCompiledAssemblyOutputPath = assemblyOutputPath.Replace("/", "\\");
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.Location.EndsWith(formattedCompiledAssemblyOutputPath));
            if (assembly == null)
                return;

            var assemblyLocation = assembly.Location;
            if (!TypesModifiedCallbacksByAssemblyLocation.ContainsKey(assemblyLocation))
                return;
            
            var compiledAssembly = CompilationPipeline.GetAssemblies()
                .FirstOrDefault(assembly => assembly.outputPath == assemblyOutputPath);
            if (compiledAssembly == null)
                return;

            var assetPaths = compiledAssembly.sourceFiles
                .Intersect(importedAssetPaths)
                .ToArray();

            string json;
            PersistentAssetPaths persistentAssetPaths;
            
            if (File.Exists(PersistentAssetPathsFilePath))
            {
                json = File.ReadAllText(PersistentAssetPathsFilePath);
                persistentAssetPaths = JsonUtility.FromJson<PersistentAssetPaths>(json);
            }
            else persistentAssetPaths = new PersistentAssetPaths();

            var persistentAssetPathsAssembly = new PersistentAssetPaths.Assembly(assemblyLocation, assetPaths);
            persistentAssetPaths.Assemblies.Add(persistentAssetPathsAssembly);

            json = JsonUtility.ToJson(persistentAssetPaths);
            File.WriteAllText(PersistentAssetPathsFilePath, json);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!File.Exists(PersistentAssetPathsFilePath))
                return;

            var json = File.ReadAllText(PersistentAssetPathsFilePath);
            var persistentAssetPaths = JsonUtility.FromJson<PersistentAssetPaths>(json);
            File.Delete(PersistentAssetPathsFilePath);

            foreach (var persistentAssetPathAssembly in persistentAssetPaths.Assemblies)
            {
                if (!TypesModifiedCallbacksByAssemblyLocation.TryGetValue(persistentAssetPathAssembly.AssemblyLocation, out var typesModifiedCallbacks))
                    continue;
                
                var types = persistentAssetPathAssembly.AssetPaths
                    .Select(
                        assetPath =>
                        {
                            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                            if (assetType != typeof(MonoScript))
                                return null;

                            var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                            return TypeExt.TryGetTypeByName(monoScript.name, out var type)
                                ? type
                                : null;
                        }
                    )
                    .Where(type => type != null)
                    .ToArray();
                
                foreach (var typesModifiedCallback in typesModifiedCallbacks)
                    typesModifiedCallback.Invoke(types);
            }
        }

        public static void AddTypesModifiedCallback(System.Reflection.Assembly assembly, TypesModifiedCallback typesModifiedCallback)
        {
            var assemblyLocation = assembly.Location;
            if (!TypesModifiedCallbacksByAssemblyLocation.TryGetValue(assemblyLocation, out var typesModifiedCallbacks))
            {
                typesModifiedCallbacks = new();
                TypesModifiedCallbacksByAssemblyLocation[assemblyLocation] = typesModifiedCallbacks;
            }
            
            typesModifiedCallbacks.Add(typesModifiedCallback);
        }

        public static void RemoveTypesModifiedCallback(System.Reflection.Assembly assembly, TypesModifiedCallback typesModifiedCallback)
        {
            var assemblyLocation = assembly.Location;
            if (!TypesModifiedCallbacksByAssemblyLocation.TryGetValue(assemblyLocation, out var typesModifiedCallbacks))
                return;
            
            typesModifiedCallbacks.Remove(typesModifiedCallback);
        }
    }
}
