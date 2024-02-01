using System;

namespace Fizz6.Data
{
    [Serializable]
    public class StringConcatConsumerProvider : ConsumerProvider<string, string, string>
    {
        protected override string Invoke(string in0, string in1) => 
            string.Concat(in0, in1);
    }
}
