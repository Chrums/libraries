using System;
using UnityEngine;

namespace Fizz6.Data
{
    public interface IConsumer : IDisposable
    {
        Type[] InputTypes { get; }
        IProvider[] Providers { get; set; }
        void Initialize(Binding binding);
    }
    
    [Serializable]
    public abstract class Consumer : IConsumer
    {
        [SerializeReference]
        private IProvider[] providers;

        public IProvider[] Providers
        {
            get => providers;
            set => providers = value;
        }
        
        public abstract Type[] InputTypes { get; }

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
        
        public abstract void Invoke();
    }

    [Serializable]
    public abstract class Consumer<TIn0> : Consumer
    {
        public override Type[] InputTypes => new[] { typeof(TIn0) };

        public override void Invoke()
        {
            var in0 = Providers[0] is IProvider<TIn0> provider0
                ? provider0.Value
                : default;
            Invoke(in0);
        }

        protected abstract void Invoke(TIn0 in0);
    }

    [Serializable]
    public abstract class Consumer<TIn0, TIn1> : Consumer
    {
        public override Type[] InputTypes => new[] { typeof(TIn0), typeof(TIn1) };
        
        public override void Invoke()
        {
            var in0 = Providers[0] is IProvider<TIn0> provider0
                ? provider0.Value
                : default;
            var in1 = Providers[0] is IProvider<TIn1> provider1
                ? provider1.Value
                : default;
            Invoke(in0, in1);
        }

        protected abstract void Invoke(TIn0 in0, TIn1 in1);
    }

    [Serializable]
    public abstract class Consumer<TIn0, TIn1, TIn2> : Consumer
    {
        public override Type[] InputTypes => new[] { typeof(TIn0), typeof(TIn1), typeof(TIn2) };
        
        public override void Invoke()
        {
            var in0 = Providers[0] is IProvider<TIn0> provider0
                ? provider0.Value
                : default;
            var in1 = Providers[1] is IProvider<TIn1> provider1
                ? provider1.Value
                : default;
            var in2 = Providers[2] is IProvider<TIn2> provider2
                ? provider2.Value
                : default;
            Invoke(in0, in1, in2);
        }

        protected abstract void Invoke(TIn0 in0, TIn1 in1, TIn2 in2);
    }

    [Serializable]
    public abstract class Consumer<TIn0, TIn1, TIn2, TIn3> : Consumer
    {
        public override Type[] InputTypes => new[] { typeof(TIn0), typeof(TIn1), typeof(TIn2), typeof(TIn3) };
        
        public override void Invoke()
        {
            var in0 = Providers[0] is IProvider<TIn0> provider0
                ? provider0.Value
                : default;
            var in1 = Providers[1] is IProvider<TIn1> provider1
                ? provider1.Value
                : default;
            var in2 = Providers[2] is IProvider<TIn2> provider2
                ? provider2.Value
                : default;
            var in3 = Providers[3] is IProvider<TIn3> provider3
                ? provider3.Value
                : default;
            Invoke(in0, in1, in2, in3);
        }

        protected abstract void Invoke(TIn0 in0, TIn1 in1, TIn2 in2, TIn3 in3);
    }
}
