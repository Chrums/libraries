using System;

namespace Fizz6.Data
{
    [Serializable]
    public class StringToUpperConsumerProvider : ConsumerProvider<string, string>
    {
        protected override string Invoke(string in0) =>
            in0.ToUpper();
    }
}
