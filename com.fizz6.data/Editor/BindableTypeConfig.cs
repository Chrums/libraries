using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [Serializable]
        public class Record
        {
            [Serializable]
            private class Item
            {
                [SerializeField] 
                private SerializableType type;
                public Type Type => type?.Value;

                [SerializeField] 
                private Bindables bindables;
                public Bindables Bindables => bindables;
            }

            [Serializable]
            public class Bindables
            {
                [SerializeField] 
                private SerializableFieldInfo[] fieldInfos;
                public IReadOnlyList<FieldInfo> FieldInfos => fieldInfos
                    .Select(fieldInfo => fieldInfo?.Value)
                    .ToArray();
                
                [SerializeField] 
                private SerializablePropertyInfo[] propertyInfos;
                public IReadOnlyList<PropertyInfo> PropertyInfos => propertyInfos
                    .Select(propertyInfo => propertyInfo?.Value)
                    .ToArray();
                
                [SerializeField] 
                private SerializableMethodInfo[] methodInfos;
                public IReadOnlyList<MethodInfo> MethodInfos => methodInfos
                    .Select(methodInfo => methodInfo?.Value)
                    .ToArray();
            }
            
            [SerializeField] 
            private SerializableType type;
            public Type Type => type?.Value;

            [SerializeField] 
            private Item[] items;
            
            private Dictionary<Type, Bindables> _data;
            public IReadOnlyDictionary<Type, Bindables> Data
            {
                get
                {
                    if (_data != null)
                        return _data;

                    _data = new();
                    foreach (var item in items)
                        _data[item.Type] = item.Bindables;

                    return _data;
                }
            }
        }
        
        [InitializeOnLoadMethod]
        private static void Register()
        {
            AssetPostprocessorManager.ImportedAssetsEvent += OnImportedAssets;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        private static IReadOnlyList<string> _importedAssetPaths;

        private static void OnImportedAssets(IReadOnlyList<string> assetPaths) =>
            _importedAssetPaths = assetPaths;

        private static void OnAssemblyCompilationFinished(string assemblyOutputPath, CompilerMessage[] compilerMessages)
        {
            var assembly = CompilationPipeline.GetAssemblies()
                .FirstOrDefault(assembly => assembly.outputPath == assemblyOutputPath);
            if (assembly == null)
                return;
            
            var formattedAssemblyOutputPath = assemblyOutputPath.Replace("/", "\\");
            if (Instance.assembly?.Value != null && !Instance.assembly.Value.Location.EndsWith(formattedAssemblyOutputPath))
                return;

            var assetPaths = assembly.sourceFiles.Intersect(_importedAssetPaths);
            _importedAssetPaths = null;

            foreach (var assetPath in assetPaths)
                TryRebuild(assetPath, out _);
        }

        [MenuItem("Fizz6/Data/Rebuild BindableTypeConfig")]
        public static void Rebuild()
        {
            var assembly = CompilationPipeline.GetAssemblies()
                .FirstOrDefault(
                    assembly =>
                    {
                        var formattedAssemblyOutputPath = assembly.outputPath.Replace("/", "\\");
                        return Instance.assembly.Value.Location.EndsWith(formattedAssemblyOutputPath);
                    }
                );
            
            if (assembly == null)
                return;

            var assetPaths = assembly.sourceFiles;
            foreach (var assetPath in assetPaths)
                TryRebuild(assetPath, out _);
        }

        private static bool TryRebuild(string assetPath, out Record record)
        {
            record = null;
            
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (assetType != typeof(MonoScript))
                return false;

            var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (!TypeExt.TryGetTypeByName(monoScript.name, out var type))
                return false;

            var modelType = typeof(IModel);
            if (!modelType.IsAssignableFrom(type))
                return false;

            var bindableType = typeof(IBindable<>);
            
            var fieldInfosByType = new Dictionary<Type, HashSet<FieldInfo>>();
            var fieldInfos = type.GetFields();
            foreach (var typeFieldInfo in fieldInfos)
            {
                var interfaceTypes = typeFieldInfo.FieldType.GetInterfaces();
                var bindableInterfaceType = interfaceTypes.FirstOrDefault(interfaceType => interfaceType.GetGenericTypeDefinition() == bindableType);
                if (bindableInterfaceType == null)
                    continue;
                
                var fieldType = bindableInterfaceType.GetGenericArguments().FirstOrDefault();
                if (fieldType == null)
                    continue;

                if (!fieldInfosByType.TryGetValue(fieldType, out var bindableFieldInfos))
                {
                    bindableFieldInfos = new HashSet<FieldInfo>();
                    fieldInfosByType[fieldType] = bindableFieldInfos;
                }

                bindableFieldInfos.Add(typeFieldInfo);
            }
            
            foreach (var (fieldType, bindableFieldInfos) in fieldInfosByType)
            {
                foreach (var bindableFieldInfo in bindableFieldInfos)
                    Debug.LogError($"{fieldType.Name}: {bindableFieldInfo.Name}");
            }
            
            var propertyInfosByType = new Dictionary<Type, HashSet<PropertyInfo>>();
            var propertyInfos = type.GetProperties();
            foreach (var typePropertyInfo in propertyInfos)
            {
                var interfaceTypes = typePropertyInfo.PropertyType.GetInterfaces();
                var bindableInterfaceType = interfaceTypes.FirstOrDefault(interfaceType => interfaceType.GetGenericTypeDefinition() == bindableType);
                if (bindableInterfaceType == null)
                    continue;
                
                var propertyType = bindableInterfaceType.GetGenericArguments().FirstOrDefault();
                if (propertyType == null)
                    continue;

                if (!propertyInfosByType.TryGetValue(propertyType, out var bindablePropertyInfos))
                {
                    bindablePropertyInfos = new HashSet<PropertyInfo>();
                    propertyInfosByType[propertyType] = bindablePropertyInfos;
                }

                bindablePropertyInfos.Add(typePropertyInfo);
            }
            
            var methodInfosByType = new Dictionary<Type, HashSet<MethodInfo>>();
            var methodInfos = type.GetMethods();
            foreach (var typeMethodInfo in methodInfos)
            {
                var interfaceTypes = typeMethodInfo.ReturnType.GetInterfaces();
                var bindableInterfaceType = interfaceTypes.FirstOrDefault(interfaceType => interfaceType.GetGenericTypeDefinition() == bindableType);
                if (bindableInterfaceType == null)
                    continue;
                
                var methodType = bindableInterfaceType.GetGenericArguments().FirstOrDefault();
                if (methodType == null)
                    continue;

                if (!methodInfosByType.TryGetValue(methodType, out var bindableMethodInfos))
                {
                    bindableMethodInfos = new HashSet<MethodInfo>();
                    methodInfosByType[methodType] = bindableMethodInfos;
                }

                bindableMethodInfos.Add(typeMethodInfo);
            }

            return true;
        }
        
        [SerializeField] 
        private SerializableAssembly assembly;
    }
}
