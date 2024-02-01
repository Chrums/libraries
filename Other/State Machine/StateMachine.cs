using System;
using System.Collections.Generic;
using Fizz6.Core;

namespace Fizz6.StateMachine
{
    public class StateMachine<TEnum> 
        where TEnum : Enum
    {
        private TEnum state;

        public TEnum State
        {
            get => state;
            set
            {
                if (state.Equals(value) || !CanChange(state, value)) return;
                ChangeEvent?.Invoke(state, value);
                state = value;
            }
        }

        public event Func<TEnum, TEnum, bool> CanChangeEvent;
        public event Action<TEnum, TEnum> ChangeEvent;

        private bool CanChange(TEnum from, TEnum to) =>
            CanChangeEvent.Aggregate(from, to);
    }

    public class StateMachine<TEnum, TState> : StateMachine<TEnum>
        where TEnum : Enum 
        where TState : StateMachine<TEnum, TState>.IImplementation
    {
        public interface IImplementation : IDisposable
        {
            void Initialize(TEnum state, StateMachine<TEnum, TState> stateMachine);
            void Enter();
            void Exit();
            void Update();
        }

        public abstract class Implementation : IImplementation
        {
            private TEnum state;
            private StateMachine<TEnum, TState> stateMachine;
            
            public void Initialize(TEnum state, StateMachine<TEnum, TState> stateMachine)
            {
                this.state = state;
                this.stateMachine = stateMachine;

                stateMachine.CanChangeEvent += OnCanChange;
                stateMachine.ChangeEvent += OnChange;
            }

            public void Dispose()
            {
                stateMachine.CanChangeEvent -= OnCanChange;
                stateMachine.ChangeEvent -= OnChange;
            }

            private bool OnCanChange(TEnum from, TEnum to) =>
                CanChangeFrom(from) &&
                CanChangeTo(to);

            private void OnChange(TEnum from, TEnum to)
            {
                if (from.Equals(state)) Exit();
                if (to.Equals(state)) Enter();
            }

            protected virtual bool CanChangeFrom(TEnum state) => true;
            protected virtual bool CanChangeTo(TEnum state) => true;
            
            public virtual void Enter()
            {}
            
            public virtual void Exit()
            {}
            
            public virtual void Update()
            {}
        }

        private readonly Dictionary<TEnum, TState> states = new();
        
        public TState this[TEnum state]
        {
            get => states[state];
            set
            {
                if (value == null)
                {
                    states[state]?.Dispose();
                }
                
                states[state] = value;

                if (value != null)
                {
                    states[state].Initialize(state, this);
                }
            }
        }

        public virtual void Update() => states[State].Update();
    }
}