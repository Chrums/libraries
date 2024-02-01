using Fizz6.SerializeImplementation;
using UnityEngine;

namespace Fizz6.UI
{
    public abstract class ImplementationController<TModel> : Controller<TModel> 
        where TModel : Model
    {
        [SerializeReference, SerializeImplementation]
        private IControllerImplementation implementation;

        protected override void OnModelBroadcast()
        {
            implementation.OnModelBroadcast();
        }
    }
}