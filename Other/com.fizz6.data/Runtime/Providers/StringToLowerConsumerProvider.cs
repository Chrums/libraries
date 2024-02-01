using System;

namespace Fizz6.Data
{
    [Serializable]
    public class StringToLowerConsumerProvider : ConsumerProvider<string, string>
    {
        protected override string Invoke(string in0) =>
            in0.ToLower();
    }
}
