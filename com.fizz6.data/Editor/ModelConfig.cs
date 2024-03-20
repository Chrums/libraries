using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fizz6.Reflection;
using UnityEngine;

namespace Fizz6.Data.Editor
{
    public class ModelConfig : ScriptableObject
    {
        [Serializable]
        public class Reference
        {
            [SerializeField] 
            private SerializableType serializableType;
            public Type Type => serializableType?.Value;

            [SerializeField]
            private SerializableMemberInfo serializableMemberInfo;
            public MemberInfo MemberInfo => serializableMemberInfo?.Value;
        }
        
        [SerializeField] 
        private SerializableType serializableType;
        public Type Type
        {
            get => serializableType;
            set => serializableType = value;
        }

        [SerializeField] 
        private SerializableMemberInfo[] serializableMemberInfos;
        
        private Dictionary<Type, IReadOnlyCollection<MemberInfo>> _memberInfos;
        public IReadOnlyDictionary<Type, IReadOnlyCollection<MemberInfo>> MemberInfos
        {
            get
            {
                if (_memberInfos != null)
                    return _memberInfos;

                var memberInfos = new Dictionary<Type, List<MemberInfo>>();
                foreach (var serializableMemberInfo in serializableMemberInfos)
                {
                    var type = serializableMemberInfo.Value.MemberType switch
                    {
                        MemberTypes.Field => serializableMemberInfo.Value is FieldInfo fieldInfo
                            ? fieldInfo.FieldType
                            : null,
                        MemberTypes.Property => serializableMemberInfo.Value is PropertyInfo propertyInfo
                            ? propertyInfo.PropertyType
                            : null,
                        _ => null
                    };
                    
                    if (type == null)
                        continue;

                    if (!memberInfos.TryGetValue(type, out var bindableMemberInfos))
                    {
                        bindableMemberInfos = new();
                        memberInfos[type] = bindableMemberInfos;
                    }
                    
                    bindableMemberInfos.Add(serializableMemberInfo.Value);
                }
                
                _memberInfos = memberInfos
                    .ToDictionary(
                        kvp => kvp.Key, 
                        kvp => kvp.Value as IReadOnlyCollection<MemberInfo>
                    );

                return _memberInfos;
            }
        }

        private Type BindableType => typeof(IBindable<>);

        public bool TryBuild()
        {
            if (Type == null)
                return false;
            
            serializableMemberInfos = Type.GetMembers()
                .Where(
                    memberInfo =>
                    {
                        var interfaceTypes = memberInfo.MemberType switch
                        {
                            MemberTypes.Field => memberInfo is FieldInfo fieldInfo 
                                ? fieldInfo.FieldType.GetInterfaces() 
                                : null,
                            MemberTypes.Property => memberInfo is PropertyInfo propertyInfo 
                                ? propertyInfo.PropertyType.GetInterfaces() 
                                : null,
                            _ => null
                        };

                        if (interfaceTypes == null)
                            return false;
            
                        var bindableInterfaceType = interfaceTypes
                            .FirstOrDefault(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == BindableType);
                        if (bindableInterfaceType == null)
                            return false;
            
                        var memberType = bindableInterfaceType.GetGenericArguments()
                            .FirstOrDefault();
                        return memberType != null;
                    }
                )
                .Select(bindableMemberInfo => new SerializableMemberInfo(bindableMemberInfo))
                .ToArray();

            return true;
        }
    }
}
