using System;
using System.Linq;
using System.Reflection;
using Fizz6.Reflection;
using UnityEngine;

namespace Fizz6.Data
{
    [Serializable]
    public class MemberInfoConsumerProvider<TOut> : ConsumerProvider<TOut>
    {
        [SerializeField]
        private Component component;
        
        [SerializeField]
        private SerializableMemberInfo memberInfo;

        public override Type[] InputTypes
        {
            get
            {
                if (memberInfo is not { Value: MethodInfo methodInfo })
                    return Array.Empty<Type>();

                var parameterInfos = methodInfo.GetParameters();
                return parameterInfos
                    .Select(parameterInfo => parameterInfo.ParameterType)
                    .ToArray();
            }
        }

        public override TOut Value
        {
            get
            {
                try
                {
                    if (memberInfo == null)
                        return default;
                    
                    switch (memberInfo.Value)
                    {
                        case FieldInfo fieldInfo:
                            return (TOut)fieldInfo.GetValue(component);
                        case PropertyInfo propertyInfo:
                            return (TOut)propertyInfo.GetValue(component);
                        case MethodInfo methodInfo:
                            var parameters = Providers
                                .Select(provider => provider.Box)
                                .ToArray();
                            return (TOut)methodInfo.Invoke(component, parameters);
                    }

                    return default;
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError(e);
                    return default;
                }
            }
        }
    }
}
