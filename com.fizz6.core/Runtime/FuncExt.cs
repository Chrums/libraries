using System;
using System.Linq;

namespace Fizz6.Core
{
    public static class FuncExt
    {
        public static bool Aggregate(this Func<bool> func)
        {
            var invocationList = func?.GetInvocationList();
            if (invocationList == null) return true;
            return invocationList
                .Aggregate(
                    true,
                    (current, invocation) => invocation.DynamicInvoke() is bool next 
                        ? current && next 
                        : current
                );
        }
        
        public static bool Aggregate<TArg0>(this Func<TArg0, bool> func, TArg0 arg0)
        {
            var invocationList = func?.GetInvocationList();
            if (invocationList == null) return true;
            return invocationList
                .Aggregate(
                    true,
                    (current, invocation) => invocation.DynamicInvoke(arg0) is bool next 
                        ? current && next 
                        : current
                );
        }
        
        public static bool Aggregate<TArg0, TArg1>(this Func<TArg0, TArg1, bool> func, TArg0 arg0, TArg1 arg1)
        {
            var invocationList = func?.GetInvocationList();
            if (invocationList == null) return true;
            return invocationList
                .Aggregate(
                    true,
                    (current, invocation) => invocation.DynamicInvoke(arg0, arg1) is bool next 
                        ? current && next 
                        : current
                );
        }
        
        public static bool Aggregate<TArg0, TArg1, TArg2>(this Func<TArg0, TArg1, TArg2, bool> func, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            var invocationList = func?.GetInvocationList();
            if (invocationList == null) return true;
            return invocationList
                .Aggregate(
                    true,
                    (current, invocation) => invocation.DynamicInvoke(arg0, arg1, arg2) is bool next 
                        ? current && next 
                        : current
                );
        }
        
        public static bool Aggregate<TArg0, TArg1, TArg2, TArg3>(this Func<TArg0, TArg1, TArg2, bool> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var invocationList = func?.GetInvocationList();
            if (invocationList == null) return true;
            return invocationList
                .Aggregate(
                    true,
                    (current, invocation) => invocation.DynamicInvoke(arg0, arg1, arg2, arg3) is bool next 
                        ? current && next 
                        : current
                );
        }
        
        public static T Aggregate<T>(this Func<T, T> func, T value)
        {
            var invocationList = func?.GetInvocationList();
            if (invocationList == null) return value;
            return invocationList
                .Aggregate(
                    value,
                    (current, invocation) => invocation.DynamicInvoke(current) is T next 
                        ? next 
                        : current
                );
        }
    }
}