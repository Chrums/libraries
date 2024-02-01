using System;

namespace Fizz6.Actor
{
    public interface IBehaviour : IDisposable
    {
        void Initialize(IActor actor);
        void Activate();
        void Deactivate();
        bool Yield(IBehaviour behaviour);
        bool Query();
        void Update();
    }

    [Serializable]
    public abstract class Behaviour : IBehaviour
    {
        private IActor actor;

        public virtual void Initialize(IActor actor)
        {
            this.actor = actor;
        }

        public virtual void Dispose()
        {
            actor = null;
        }
        
        public virtual void Activate()
        {}
        
        public virtual void Deactivate()
        {}
        
        public virtual bool Yield(IBehaviour behaviour) => false;

        protected void Yield()
        {
            actor.Yield(this);
        }
        
        public abstract bool Query();
        public abstract void Update();
    }
}