using System;
using Fizz6.SerializeImplementation;
using UnityEngine;

namespace Fizz6.Data
{
    public interface IProvider
    {
        Type OutputType { get; }
        object Box { get; }
    }

    public interface IProvider<out TOut> : IProvider
    {
        TOut Value { get; }
    }
    
    [Serializable]
    public abstract class Provider<TOut> : IProvider<TOut>
    {
        public Type OutputType => typeof(TOut);
        public object Box => Value;
        public abstract TOut Value { get; }
    }

    [Serializable]
    public abstract class ConsumerProvider<TOut> : IConsumer, IProvider<TOut>
    {
        [SerializeReference]
        private IProvider[] providers;

        public IProvider[] Providers
        {
            get => providers;
            set => providers = value;
        }
        
        public abstract Type[] InputTypes { get; }
        public Type OutputType => typeof(TOut);
        
        public object Box => Value;
        public abstract TOut Value { get; }
        
        protected Binding Binding { get; private set; }

        public virtual void Initialize(Binding binding)
        {
            Binding = binding;
            foreach (var provider in Providers)
                if (provider is IConsumer consumer)
                    consumer.Initialize(binding);
        }

        public virtual void Dispose()
        {
            Binding = null;
            foreach (var provider in Providers)
                if (provider is IConsumer consumer)
                    consumer.Dispose();
        }
    }

    [Serializable]
    public abstract class ConsumerProvider<TIn0, TOut> : ConsumerProvider<TOut>
    {
        public override Type[] InputTypes => new[] { typeof(TIn0) };
        
        public override TOut Value
        {
            get
            {
                if (Providers == null)
                    return default;
                
                var in0 = Providers[0] is IProvider<TIn0> provider0
                    ? provider0.Value
                    : default;
                return Invoke(in0);
            }
        }

        protected abstract TOut Invoke(TIn0 in0);
    }
    
    [Serializable]
    public abstract class ConsumerProvider<TIn0, TIn1, TOut> : ConsumerProvider<TOut>
    {
        public override Type[] InputTypes => new[] { typeof(TIn0), typeof(TIn1) };
        
        public override TOut Value
        {
            get
            {
                if (Providers == null)
                    return default;
                
                var in0 = Providers[0] is IProvider<TIn0> provider0
                    ? provider0.Value
                    : default;
                var in1 = Providers[1] is IProvider<TIn1> provider1
                    ? provider1.Value
                    : default;
                return Invoke(in0, in1);
            }
        }

        protected abstract TOut Invoke(TIn0 in0, TIn1 in1);
    }
}