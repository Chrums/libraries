using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableAssembly
    {
        [SerializeField]
        private string fullName;
        
        [NonSerialized]
        private Assembly _value;
        public Assembly Value
        {
            get
            {
                if (string.IsNullOrEmpty(fullName))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(assembly => assembly.FullName == fullName);
                return _value;
            }
            
            set
            {
                fullName = value.FullName;
                _value = value;
            }
        }

        public SerializableAssembly(Assembly value)
        {
            fullName = value.FullName;
            _value = value;
        }

        public static implicit operator SerializableAssembly(Assembly value) =>
            new(value);

        public static implicit operator Assembly(SerializableAssembly value) =>
            value.Value;
    }
}
