using UnityEngine;

namespace Fizz6.SerializeImplementation
{
    public abstract class SerializeImplementationMonoBehaviour<T> : MonoBehaviour 
        where T : IMonoBehaviourSerializeImplementation
    {
        [SerializeReference, SerializeImplementation]
        private T implementation;

        protected virtual void Awake()
        {
            implementation.MonoBehaviour = this;
            implementation.Awake();
        }
        
        protected virtual void Start() =>
            implementation.Start();

        protected virtual void OnDestroy()
        {
            implementation.MonoBehaviour = null;
            implementation.OnDestroy();
        }

        protected virtual void OnEnable() =>
            implementation.OnEnable();
        
        protected virtual void OnDisable() =>
            implementation.OnDisable();
        
        protected virtual void Update() =>
            implementation.Update();
    }
}