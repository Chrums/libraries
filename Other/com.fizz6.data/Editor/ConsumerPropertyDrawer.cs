using System;
using System.Linq;
using Fizz6.Core;
using Fizz6.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Fizz6.Data.Editor
{
    [CustomPropertyDrawer(typeof(IConsumer), true)]
    public class ConsumerPropertyDrawer : PropertyDrawer
    {
        private const string ProvidersFieldName = "providers";
        private static SerializedProperty TryGetProviderSerializedProperty(SerializedProperty serializedProperty, int index) =>
            serializedProperty.FindPropertyRelative($"{ProvidersFieldName}.Array.data[{index}]");
        
        public static Rect ProvidersOnGUI(Rect position, SerializedProperty property)
        {
            var consumer = property.GetValue<IConsumer>();
            for (var index = 0; index < consumer.Providers.Length; ++index)
            {
                var inputType = consumer.InputTypes[index];
                var provider = consumer.Providers[index];
                var assignableTypes = typeof(IProvider<>)
                    .MakeGenericType(inputType)
                    .GetAssignableTypes();
            
                var selectedIndex = int.MinValue;
                if (provider != null)
                {
                    for (var count = 0; count < assignableTypes.Count; ++count)
                    {
                        var assignableType = assignableTypes[count];
                        if (provider.GetType() != assignableType) 
                            continue;
                        selectedIndex = count;
                        break;
                    }
                }
            
                var displayedOptions = assignableTypes
                    .Select(assignableType => assignableType.Name)
                    .ToArray();
            
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    position.height = EditorGUIUtility.singleLineHeight;
                    selectedIndex = EditorGUI.Popup(position, selectedIndex, displayedOptions);
                    if (changeCheckScope.changed)
                        consumer.Providers[index] = Activator.CreateInstance(assignableTypes[selectedIndex]) as IProvider;
                    position.y = position.yMax + SerializedPropertyLayout.PropertyPadding;
                }

                var providerSerializedProperty = TryGetProviderSerializedProperty(property, index);

                if (providerSerializedProperty == null)
                    continue;
            
                var providerChildrenSerializedProperties = providerSerializedProperty
                    .GetChildren();
            
                if (!providerChildrenSerializedProperties.Any()) 
                    continue;
            
                position.x += EditorGUIUtility.singleLineHeight;
                position.width -= EditorGUIUtility.singleLineHeight;
            
                var propertyValueType = providerSerializedProperty.GetValueType();
                if (propertyValueType != null && propertyValueType.TryGetPropertyDrawer(out var propertyDrawerType))
                {
                    var propertyDrawer = (PropertyDrawer)Activator.CreateInstance(propertyDrawerType);
                    position.height = propertyDrawer.GetPropertyHeight(providerSerializedProperty, null);
                    propertyDrawer.OnGUI(position, providerSerializedProperty, null);
                    position.y = position.yMax + SerializedPropertyLayout.PropertyPadding;
                }
                else
                {
                    foreach (var providerChildSerializedProperty in providerChildrenSerializedProperties)
                    {
                        position.height = EditorGUI.GetPropertyHeight(providerChildSerializedProperty);
                        EditorGUI.PropertyField(position, providerChildSerializedProperty, true);
                        position.y = position.yMax + SerializedPropertyLayout.PropertyPadding;
                    }
                }

                position.x -= EditorGUIUtility.singleLineHeight;
                position.width += EditorGUIUtility.singleLineHeight;
            }

            return position;
        }

        public static float ProvidersGetPropertyHeight(SerializedProperty property)
        {
            var height = 0.0f;
            
            var consumer = property.GetValue<IConsumer>();

            if (consumer.Providers == null)
                return height;
            
            for (var index = 0; index < consumer.Providers.Length; ++index)
            {
                height += EditorGUIUtility.singleLineHeight + SerializedPropertyLayout.PropertyPadding;
                
                var providerSerializedProperty = TryGetProviderSerializedProperty(property, index);
                
                if (providerSerializedProperty == null)
                    continue;
            
                var providerChildrenSerializedProperties = providerSerializedProperty
                    .GetChildren();
            
                if (!providerChildrenSerializedProperties.Any()) 
                    continue;
                
                var propertyValueType = providerSerializedProperty.GetValueType();
                if (propertyValueType != null && propertyValueType.TryGetPropertyDrawer(out var propertyDrawerType))
                {
                    var propertyDrawer = (PropertyDrawer)Activator.CreateInstance(propertyDrawerType);
                    height += propertyDrawer.GetPropertyHeight(providerSerializedProperty, null);
                }
                else
                    height += providerChildrenSerializedProperties.Sum(EditorGUI.GetPropertyHeight);

                height += SerializedPropertyLayout.PropertyPadding;
            }

            return height;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);

            var consumer = property.GetValue<IConsumer>();

            if (consumer.Providers == null)
                consumer.Providers = new IProvider[consumer.InputTypes.Length];
            else if (consumer.Providers.Length != consumer.InputTypes.Length)
            {
                var providers = consumer.Providers;
                Array.Resize(ref providers, consumer.InputTypes.Length);
                consumer.Providers = providers;
            }

            position = ProvidersOnGUI(position, property);
            
            var childrenSerializedProperties = property
                .GetChildren();
            
            if (!childrenSerializedProperties.Any()) 
                return;
            
            foreach (var serializedProperty in childrenSerializedProperties)
            {
                if (serializedProperty.GetValue() == consumer.Providers)
                    continue;
                position.height = EditorGUI.GetPropertyHeight(serializedProperty);
                EditorGUI.PropertyField(position, serializedProperty, true);
                position.y = position.yMax + SerializedPropertyLayout.PropertyPadding;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // return base.GetPropertyHeight(property, label);
            
            var height = 0.0f;
            
            var consumer = property.GetValue<IConsumer>();

            if (consumer.Providers == null)
                return height;

            height += ProvidersGetPropertyHeight(property);
                
            var childrenSerializedProperties = property
                .GetChildren();
            
            if (!childrenSerializedProperties.Any()) 
                return height;
            
            foreach (var serializedProperty in childrenSerializedProperties)
            {
                if (serializedProperty.GetValue() == consumer.Providers)
                    continue;
                height += EditorGUI.GetPropertyHeight(serializedProperty) +
                          SerializedPropertyLayout.PropertyPadding;
            }

            return height;
        }
    }
}