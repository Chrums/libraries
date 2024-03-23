using System;

namespace Fizz6.Data
{
    public interface IInvokableBindableSubscription : Bindings.ISubscription
    {
        event Action InvokeEvent;
    }
    
    public interface IInvokableBindable : IBindable
    {
        void Invoke();
    }

    public class InvokableBindable : Bindable, IInvokableBindable
    {
        private Action _invoke;
        
        public InvokableBindable(IModel model, string memberName, Action invoke) : base(model, memberName)
        {
            _invoke = invoke;
            
            Bindings.Bind(this);
        }
        
        ~InvokableBindable()
        {
            Bindings.Unbind(this);

            _invoke = null;
        }

        public override void Clear()
        {}

        public void Invoke() =>
            _invoke?.Invoke();
    }
}