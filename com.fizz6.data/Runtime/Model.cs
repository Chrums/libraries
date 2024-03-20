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
        protected virtual IReadOnlyList<IBindable> Bindables { get; }

        protected virtual void Awake() =>
            _bindables = Bindables;
        
        protected virtual void OnDestroy()
        {
            foreach (var bindable in _bindables)
                bindable.Unbind();
        }

        public void Clear()
        {
            foreach (var bindable in _bindables)
                bindable.Clear();
        }
    }
}
