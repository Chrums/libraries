using System;
using UnityEngine;

namespace Fizz6.UI
{
    public interface IModel
    {
        event Action BroadcastEvent;
    }
    
    public abstract class Model : MonoBehaviour, IModel
    {
        public event Action BroadcastEvent;

        protected virtual void Awake()
        {
            var controllers = GetComponentsInChildren<IController>();
            foreach (var controller in controllers)
            {
                controller.TryInitialize();
            }
        }

        protected virtual void OnDestroy() {}
        
        protected void Broadcast() => BroadcastEvent?.Invoke();
    }
}