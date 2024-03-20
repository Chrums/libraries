using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Core.Editor
{
    public static class PropertyDrawerExt
    {
        private static readonly Dictionary<Type, Type> PropertyDrawersByType = new();
        
        public static bool TryGetPropertyDrawer(this Type type, out Type propertyDrawerType)
        {
            if (PropertyDrawersByType.TryGetValue(type, out propertyDrawerType))
                return true;
            
            PropertyDrawersByType.Clear();
            foreach (var valueType in typeof(PropertyDrawer).GetAssignableTypes())
            {
                var customPropertyDrawerAttribute = valueType.GetAttribute<CustomPropertyDrawer>();
                if (customPropertyDrawerAttribute == null)
                    continue;
                
                var keyType = typeof(CustomPropertyDrawer).TryGetPrivateFieldValue<Type>(customPropertyDrawerAttribute, "m_Type");
                
                // If multiple CustomPropertyDrawer exist for the same type,
                // use the CustomPropertyDrawer that is closer in the inheritance chain to the type
                if (PropertyDrawersByType.TryGetValue(keyType, out var currentValueType) && currentValueType != valueType)
                {
                    var currentCustomPropertyDrawerAttribute = currentValueType.GetAttribute<CustomPropertyDrawer>();
                    var currentType = typeof(CustomPropertyDrawer).TryGetPrivateFieldValue<Type>(currentCustomPropertyDrawerAttribute, "m_Type");
                    
                    for (var iteratorType = keyType; iteratorType != null; iteratorType = iteratorType.BaseType)
                    {
                        
                        // TODO: There are issues with resolving which to use when one of the types is an Interface,
                        // we still need to work out an ideal solution
                        
                        // if (valueType.IsInterface)
                        // {
                        //     
                        // }
                        //
                        // var iteratorTypeInterfaces = iteratorType.GetInterfaces();
                        // var baseTypeInterfaces = iteratorType.BaseType != null
                        //     ? iteratorType.BaseType.GetInterfaces()
                        //     : Type.EmptyTypes;
                        
                        if (iteratorType == currentType)
                            break;

                        if (iteratorType != keyType) 
                            continue;
                        
                        PropertyDrawersByType.Remove(keyType);
                        break;
                    }
                }
                
                PropertyDrawersByType.TryAdd(keyType, valueType);
                
                var useForChildren = typeof(CustomPropertyDrawer).TryGetPrivateFieldValue<bool>(customPropertyDrawerAttribute, "m_UseForChildren");
                if (!useForChildren)
                    continue;

                foreach (var childKeyType in keyType.GetAssignableTypes())
                {
                    //childKeyType.BaseType;
                    PropertyDrawersByType.TryAdd(childKeyType, valueType);
                    if (PropertyDrawersByType.TryGetValue(childKeyType, out var test2) && test2 != valueType)
                    {
                        Debug.LogError($"child: {childKeyType.Name}: {test2.Name} - {valueType.Name}");
                    }
                }
            }

            return PropertyDrawersByType.TryGetValue(type, out propertyDrawerType);
        }
    }
}
