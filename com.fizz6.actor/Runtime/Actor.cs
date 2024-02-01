using System;
using System.Collections.Generic;
using System.Linq;
using Fizz6.SerializeImplementation;
using UnityEngine;

namespace Fizz6.Actor
{
    public interface IActor : IDisposable
    {
        void Initialize();
        void Update();
        void Yield(IBehaviour behaviour);
    }
    
    [Serializable]
    public class Actor<TBehaviour> : IActor
        where TBehaviour : class, IBehaviour
    {
        [NonSerialized]
        private TBehaviour active;
        public TBehaviour Active
        {
            get => active;
            set
            {
                if (active == value) return;
                active?.Deactivate();
                active = value;
                active?.Activate();
            }
        }
        
        [SerializeReference, SerializeImplementation]
        private List<TBehaviour> behaviours = new();
        public IReadOnlyList<TBehaviour> Behaviours => behaviours;

        public virtual void Initialize()
        {
            foreach (var behaviour in behaviours) behaviour.Initialize(this);
        }

        public virtual void Dispose()
        {}

        public void Update()
        {
            Active ??= behaviours.FirstOrDefault(behaviour => behaviour.Query());
            Active = behaviours.FirstOrDefault(behaviour => behaviour.Yield(Active)) ?? Active;
            Active?.Update();
        }

        public void Yield(IBehaviour behaviour)
        {
            if (Active == behaviour) Active = null;
        }

        public void Add(TBehaviour behaviour)
        {
            behaviour.Initialize(this);
            behaviours.Add(behaviour);
        }

        public void Remove(TBehaviour behaviour)
        {
            Yield(behaviour);
            behaviours.Remove(behaviour);
            behaviour.Dispose();
        }
    }
}