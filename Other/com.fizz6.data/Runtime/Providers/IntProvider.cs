using System;
using UnityEngine;

namespace Fizz6.Data
{
    [Serializable]
    public class IntProvider : Provider<int>
    {
        [SerializeField] 
        private int value;
        public override int Value => value;
    }
}
