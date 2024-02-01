using UnityEngine;

namespace Fizz6.UI
{
    public interface IController
    {
        void TryInitialize();
    }
    
    public abstract class Controller<TModel> : MonoBehaviour, IController 
        where TModel : Model
    {
        public TModel Model { get; private set; }
        
        private bool isInitialized = false;
        
        protected virtual void Awake() =>
            TryInitialize();

        protected virtual void OnDestroy()
        {
            if (!Model) return;
            Model.BroadcastEvent -= OnModelBroadcast;
        }

        public void TryInitialize()
        {
            if (isInitialized) return;
            isInitialized = true;
            Model = GetComponentInParent<TModel>();
            Model.BroadcastEvent += OnModelBroadcast;
        }

        protected abstract void OnModelBroadcast();
    }
}