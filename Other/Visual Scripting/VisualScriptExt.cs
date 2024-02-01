using System;
using Fizz6.VisualScripting;

public static class VisualScriptExt
{
    
    #region ToAction
    
    public static Action ToAction(this VisualScriptInput visualScriptInput) =>
        () => visualScriptInput.Invoke();
    
    public static Action<TArg0> ToAction<TArg0>(this VisualScriptInput visualScriptInput) => 
        arg0 => visualScriptInput.Invoke(arg0);
    
    public static Action<TArg0, TArg1> ToAction<TArg0, TArg1>(this VisualScriptInput visualScriptInput) => 
        (arg0, arg1) => visualScriptInput.Invoke(arg0, arg1);
    
    public static Action<TArg0, TArg1, TArg2> ToAction<TArg0, TArg1, TArg2>(this VisualScriptInput visualScriptInput) => 
        (arg0, arg1, arg2) => visualScriptInput.Invoke(arg0, arg1, arg2);
    
    public static Action<TArg0, TArg1, TArg2, TArg3> ToAction<TArg0, TArg1, TArg2, TArg3>(this VisualScriptInput visualScriptInput) => 
        (arg0, arg1, arg2, arg3) => visualScriptInput.Invoke(arg0, arg1, arg2, arg3);
    
    #endregion
    
    #region ToFunc
    
    public static Func<TResult> ToFunc<TResult>(this VisualScriptFunction visualScriptFunction)
    {
        return () =>
        {
            var data = visualScriptFunction.Invoke();
            return data.Length == 1 && data[0] is TResult result
                ? result
                : default;
        };
    }
        
    public static Func<TArg0, TResult> ToFunc<TArg0, TResult>(this VisualScriptFunction visualScriptFunction)
    {
        return arg0 =>
        {
            var data = visualScriptFunction.Invoke(arg0);
            return data.Length == 1 && data[0] is TResult result
                ? result
                : default;
        };
    }
        
    public static Func<TArg0, TArg1, TResult> ToFunc<TArg0, TArg1, TResult>(this VisualScriptFunction visualScriptFunction)
    {
        return (arg0, arg1) =>
        {
            var data = visualScriptFunction.Invoke(arg0, arg1);
            return data.Length == 1 && data[0] is TResult result
                ? result
                : default;
        };
    }
    
    public static Func<TArg0, TArg1, TArg2, TResult> ToFunc<TArg0, TArg1, TArg2, TResult>(this VisualScriptFunction visualScriptFunction)
    {
        return (arg0, arg1, arg2) =>
        {
            var data = visualScriptFunction.Invoke(arg0, arg1, arg2);
            return data.Length == 1 && data[0] is TResult result
                ? result
                : default;
        };
    }
    
    public static Func<TArg0, TArg1, TArg2, TArg3, TResult> ToFunc<TArg0, TArg1, TArg2, TArg3, TResult>(this VisualScriptFunction visualScriptFunction)
    {
        return (arg0, arg1, arg2, arg3) =>
        {
            var data = visualScriptFunction.Invoke(arg0, arg1, arg2, arg3);
            return data.Length == 1 && data[0] is TResult result
                ? result
                : default;
        };
    }
    
    #endregion
    
}