namespace Fizz6.Data
{
    public interface IBindable
    {
        public IModel Model { get; }
        public string MemberName { get; }
        void Clear();
    }
    
    public abstract class Bindable : IBindable
    {
        public IModel Model { get; private set; }
        public string MemberName { get; private set; }

        protected Bindable(IModel model, string memberName)
        {
            Model = model;
            MemberName = memberName;
        }

        ~Bindable()
        {
            Model = null;
            MemberName = null;
        }

        public abstract void Clear();
    }
}
