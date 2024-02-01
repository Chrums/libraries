using UnityEngine;

namespace Fizz6.SerializeImplementation
{
    public interface IMonoBehaviourSerializeImplementation
    {
        public MonoBehaviour MonoBehaviour { get; set; }
        
        public void Awake();
        public void Start();
        public void OnDestroy();
        public void OnEnable();
        public void OnDisable();
        public void Update();
    }
}