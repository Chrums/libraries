using System;
using Fizz6.SerializeImplementation;
using UnityEngine;

namespace Fizz6.Data
{
    public class Binding : MonoBehaviour
    {
        public event Action StartEvent;
        public event Action OnEnableEvent;
        public event Action OnDisableEvent;
        
        [SerializeReference, SerializeImplementation]
        private Consumer consumer;

        private void Awake() =>
            consumer.Initialize(this);

        private void OnDestroy() =>
            consumer.Dispose();

        private void Start() =>
            StartEvent?.Invoke();

        private void OnEnable() =>
            OnEnableEvent?.Invoke();

        private void OnDisable() =>
            OnDisableEvent?.Invoke();

        public void Invoke() =>
            consumer.Invoke();
    }
}
