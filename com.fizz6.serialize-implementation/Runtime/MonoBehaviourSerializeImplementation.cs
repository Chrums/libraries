using UnityEngine;

namespace Fizz6.SerializeImplementation
{
    public class MonoBehaviourSerializeImplementation : IMonoBehaviourSerializeImplementation
    {
        private MonoBehaviour _monoBehaviour;

        public MonoBehaviour MonoBehaviour
        {
            get => _monoBehaviour;
            set => _monoBehaviour = value;
        }
        
        public virtual void Awake()
        {}
        
        public virtual void Start()
        {}
        
        public virtual void OnDestroy()
        {}
        
        public virtual void OnEnable()
        {}
        
        public virtual void OnDisable()
        {}
        
        public virtual void Update()
        {}
    }
}