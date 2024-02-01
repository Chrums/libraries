using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fizz6.Core;
using Fizz6.Core.Editor;
using UnityEngine;

namespace Fizz6.Reflection.Editor
{
    [CreateAssetMenu(fileName = "ReflectionConfig", menuName = "Fizz6/Reflection/Reflection Config")]
    public class ReflectionConfig : AssetDatabaseSingletonScriptableObject<ReflectionConfig>
    {
        [SerializeField] 
        private BindingFlags bindingFlags = BindingFlags.Instance;
        public BindingFlags BindingFlags => bindingFlags;
        
        [SerializeField]
        private SerializableAssembly[] assemblies;
        public IReadOnlyList<SerializableAssembly> Assemblies => assemblies;

        public IEnumerable<Type> Types => Assemblies.Count == 0
            ? TypeExt.AllTypesInCurrentDomain
            : Assemblies.SelectMany(serializableAssembly => serializableAssembly.Value.GetTypes())
                .ToArray();
    }
}
