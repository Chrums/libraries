using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Fizz6.Scheduler
{
    public static class ObjectExt
    {
        public static Task WaitUntilFixedUpdate(this Object context, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilFixedUpdate(context, cancellationToken);
        
        public static Task WaitUntilFixedUpdate(this Object context, int frames, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilFixedUpdate(context, cancellationToken, frames);
        
        public static Task WaitUntilFixedUpdate(this Object context, float time, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilFixedUpdate(context, cancellationToken, time);
        
        public static Task WaitUntilUpdate(this Object context, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilUpdate(context, cancellationToken);
        
        public static Task WaitUntilUpdate(this Object context, int frames, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilUpdate(context, cancellationToken, frames);
        
        public static Task WaitUntilUpdate(this Object context, float time, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilUpdate(context, cancellationToken, time);
        
        public static Task WaitUntilLateUpdate(this Object context, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilLateUpdate(context, cancellationToken);
        
        public static Task WaitUntilLateUpdate(this Object context, int frames, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilLateUpdate(context, cancellationToken, frames);
        
        public static Task WaitUntilLateUpdate(this Object context, float time, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilLateUpdate(context, cancellationToken, time);
        
        public static Task WaitUntilEndOfFrame(this Object context, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilEndOfFrame(context, cancellationToken);
        
        public static Task WaitUntilEndOfFrame(this Object context, int frames, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilEndOfFrame(context, cancellationToken, frames);
        
        public static Task WaitUntilEndOfFrame(this Object context, float time, CancellationToken cancellationToken = default) =>
            Scheduler.Instance.WaitUntilEndOfFrame(context, cancellationToken, time);
    }
}