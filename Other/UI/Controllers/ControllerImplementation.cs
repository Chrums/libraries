using System;

namespace Fizz6.UI
{
    public interface IControllerImplementation
    {
        void OnModelBroadcast();
    }
    
    [Serializable]
    public abstract class ControllerImplementation<TModel> : IControllerImplementation where TModel : Model
    {
        private Controller<TModel> controller;
        
        protected TModel Model => controller.Model;

        public void Initialize(Controller<TModel> controller)
        {
            this.controller = controller;
        }
        
        public abstract void OnModelBroadcast();
    }
}