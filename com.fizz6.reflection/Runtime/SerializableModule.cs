using System;
using System.Reflection;
using UnityEngine;

namespace Fizz6.Reflection
{
    [Serializable]
    public class SerializableModule
    {
        [SerializeField]
        private SerializableAssembly assembly;
        
        [SerializeField]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                _value = null;
            }
        }

        [NonSerialized] 
        private Module _value;
        public Module Value
        {
            get
            {
                if (assembly.Value == null || string.IsNullOrEmpty(name))
                    return null;
                
                if (_value != null) 
                    return _value;

                _value = assembly.Value.GetModule(name);
                return _value;
            }

            set
            {
                assembly = new SerializableAssembly(value.Assembly);
                name = value.Name;
                _value = value;
            }
        }
        
        public SerializableModule(Module value)
        {
            assembly = value.Assembly;
            name = value.Name;
            _value = value;
        }
            
        public static implicit operator SerializableModule(Module value) => 
            new(value);

        public static implicit operator Module(SerializableModule value) =>
            value.Value;
    }
}
