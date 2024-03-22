using System.Collections.Generic;
using UnityEngine;

namespace Fizz6.Data
{
    public interface IModel
    {
        Component Component { get; }
    }
    
    public abstract class Model : MonoBehaviour, IModel
    {
        public Component Component => this;
        
        private IReadOnlyList<IBindable> _bindables;
        protected abstract IReadOnlyList<IBindable> Bindables { get; }

        protected virtual void Awake() =>
            _bindables = Bindables;

        public void Clear()
        {
            foreach (var bindable in _bindables)
                bindable.Clear();
        }
    }
}
