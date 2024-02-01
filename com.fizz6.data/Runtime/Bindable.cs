using System;
using System.Collections.Generic;

namespace Fizz6.Data
{
    public interface IBindable<TOut>
    {}

    public class Bindable<TOut> : IBindable<TOut>
    {
        private TOut _value;
        public TOut Value
        {
            get => _value;
            set
            {
                var isEqual = EqualityComparer<TOut>.Default.Equals(_value, value);
                if (isEqual)
                    return;
                _value = value;
                ValueChangedEvent?.Invoke();
            }
        }
        
        public event Action ValueChangedEvent;
        
        public Bindable()
        {}

        public Bindable(TOut value) =>
            Value = value;
    }
}
