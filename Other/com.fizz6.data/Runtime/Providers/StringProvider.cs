using System;
using UnityEngine;

namespace Fizz6.Data
{
    [Serializable]
    public class StringProvider : Provider<string>
    {
        [SerializeField] 
        private string value;
        public override string Value => value;
    }
}
    