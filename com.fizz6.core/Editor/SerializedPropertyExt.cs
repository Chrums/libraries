using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Core.Editor
{
    public static class SerializedPropertyExt
    {
        private const char SpaceDelimiter = ' ';
        private const char PathDelimiter = '.';
        private const string ArrayDelimiter = ".Array.data";
        private static readonly Regex ArrayRegex = new Regex(@"([^\.]*?)\[(\d+)\]");
        // private static readonly Regex ArrayRegex = new Regex(@"([^\.]*?)\.Array\.data\[(\d+)\]");
        
        public static string GetManagedReferenceFullTypeName(this SerializedProperty serializedProperty)
        {
            return serializedProperty.managedReferenceFullTypename
                .Split(SpaceDelimiter)
                .Last();
        }
        
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty serializedProperty)
        {
            var iterator = serializedProperty.Copy();
            var terminator = serializedProperty.GetEndProperty();
            
            if (!iterator.NextVisible(enterChildren: true)) yield break;
            
            do
            {
                if (SerializedProperty.EqualContents(iterator, terminator)) yield break;
                yield return iterator;
            }
            while (iterator.NextVisible(enterChildren: false));
        }

        public static T GetValue<T>(this SerializedProperty serializedProperty)
        {
            return (T)GetValue(serializedProperty);
        }

        public static object GetValue(this SerializedProperty serializedProperty)
        {
            return GetValue(serializedProperty.serializedObject.targetObject, serializedProperty.propertyPath);
        }
        
        private static object GetValue(object target, string path)
        {
            var value = target;

            var elements = path.Replace(ArrayDelimiter, string.Empty)
                .Split(PathDelimiter);

            foreach (var element in elements)
            {
                var match = ArrayRegex.Match(element);
                if (!match.Success)
                {
                    var name = element;
                    value = GetValueInternal(value, name);
                }
                else
                {
                    var name = match.Groups[1].Value;
                    var index = System.Convert.ToInt32(match.Groups[2].Value);
                    value = GetArrayValueInternal(value, name, index);
                }
            }

            return value;
        }

        private static object GetValueInternal(object target, string name)
        {
            if (target == null) return null;

            var type = target.GetType();
            
            while (type != null)
            {
                var fieldInfo = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(target);
                }

                var propertyInfo = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(target, null);
                }

                type = type.BaseType;
            }
            
            return null;
        }

        private static object GetArrayValueInternal(object source, string name, int index)
        {
            if (!(GetValueInternal(source, name) is IEnumerable enumerable)) 
                return null;

            var enumerator = enumerable.GetEnumerator();

            for (var count = 0; count <= index; count++)
            {
                if (!enumerator.MoveNext()) 
                    return null;
            }

            return enumerator.Current;
        }

        public static Type GetValueType(this SerializedProperty serializedProperty) =>
            serializedProperty?.GetValue()?.GetType();
    }
}