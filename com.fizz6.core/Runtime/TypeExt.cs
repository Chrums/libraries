using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fizz6.Core
{
    public static class TypeExt
    {
        private static List<Assembly> _allAssembliesInCurrentDomain;

        public static IReadOnlyList<Assembly> AllAssembliesInCurrentDomain
        {
            get
            {
                if (_allAssembliesInCurrentDomain != null)
                    return _allAssembliesInCurrentDomain;

                _allAssembliesInCurrentDomain = AppDomain.CurrentDomain.GetAssemblies()
                    .ToList();

                return _allAssembliesInCurrentDomain;
            }
        }

        private static List<Type> _allTypesInCurrentDomain;
        public static IReadOnlyList<Type> AllTypesInCurrentDomain
        {
            get
            {
                if (_allTypesInCurrentDomain != null)
                    return _allTypesInCurrentDomain;
                
                _allTypesInCurrentDomain = AllAssembliesInCurrentDomain
                    .SelectMany(assembly => assembly.GetTypes())
                    .ToList();

                return _allTypesInCurrentDomain;
            }
        }
        
        private static readonly Dictionary<string, Type> TypesByName = new();

        public static bool TryGetTypeByName(string name, out Type type)
        {
            if (TypesByName.TryGetValue(name, out type)) 
                return true;
            
            type = AllTypesInCurrentDomain
                .FirstOrDefault(type => type.Name == name);

            if (type == null)
                return false;
            
            TypesByName[name] = type;
            return true;
        }

        private static readonly Dictionary<string, Type> TypesByFullName = new();
        
        public static bool TryGetTypeByFullName(string name, out Type type)
        {
            if (TypesByFullName.TryGetValue(name, out type)) 
                return true;
            
            type = AllTypesInCurrentDomain
                .FirstOrDefault(type => type.FullName == name);

            if (type == null)
                return false;
            
            TypesByFullName[name] = type;
            return true;
        }

        private static readonly Dictionary<Type, IReadOnlyList<Type>> AssignableTypesByType = new();
        
        public static IReadOnlyList<Type> GetAssignableTypes(this Type type)
        {
            if (AssignableTypesByType.TryGetValue(type, out var types)) 
                return types;
            
            types = AllTypesInCurrentDomain
                .Where(_ => type.IsAssignableFrom(_) && !_.IsAbstract && !_.IsInterface)
                .ToList();
            AssignableTypesByType[type] = types;

            return types;
        }

        private static readonly Dictionary<Type, IReadOnlyList<Type>> SubclassTypesByType = new();
        
        public static IReadOnlyList<Type> GetSubclassTypes(this Type type)
        {
            if (SubclassTypesByType.TryGetValue(type, out var types)) 
                return types;

            types = AllTypesInCurrentDomain
                .Where(_ => _.IsSubclassOf(type) && !_.IsAbstract && !_.IsInterface)
                .ToList();
            SubclassTypesByType[type] = types;

            return types;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute =>
            type.GetCustomAttributes()
                .FirstOrDefault(attribute => attribute is T) as T;

        public static T TryGetPrivateFieldValue<T>(this Type type, object obj, string name)
        {
            var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                throw new NullReferenceException();
            return (T)fieldInfo.GetValue(obj);
        }

        public static T TryGetPrivatePropertyValue<T>(this Type type, object obj, string name)
        {
            var propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
                throw new NullReferenceException();
            return (T)propertyInfo.GetValue(obj);
        }

        public static string GetFriendlyName(this Type type) =>
            $"{type.Name.Split("`").FirstOrDefault()}{(type.IsGenericType ? $"<{string.Join(", ", type.GetGenericArguments().Select(type => type.GetFriendlyName()))}>" : string.Empty)}";
    }
}