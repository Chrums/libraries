using System;

namespace Fizz6.Data
{
    [Serializable]
    public class IntToStringConsumerProvider : ConsumerProvider<int, string>
    {
        protected override string Invoke(int in0) =>
            in0.ToString();
    }
}
