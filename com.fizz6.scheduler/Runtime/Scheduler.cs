using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Fizz6.Scheduler
{
    public class Scheduler : SingletonMonoBehaviour<Scheduler>
    {
        private class Item
        {
            private readonly object _context;
            private readonly TaskCompletionSource<object> _taskCompletionSource;
            public Task Task => _taskCompletionSource.Task;
            
            public Item(object context, CancellationToken cancellationToken)
            {
                _context = context;
                _taskCompletionSource = new TaskCompletionSource<object>(null);
                cancellationToken.Register(_taskCompletionSource.SetCanceled);
            }
            
            public void Invoke()
            {
                if (_taskCompletionSource.Task.IsCompleted) return;
                
                if (_context == null)
                {
                    _taskCompletionSource.SetCanceled();
                    return;
                }
            
                Execute();
            }
            
            protected virtual void Execute()
            {
                _taskCompletionSource.SetResult(null);
            }
        }
        
        private class FrameItem : Item
        {
            private readonly int _frame;
            private readonly int _frames;
            
            public FrameItem(object context, CancellationToken cancellationToken, int frames) : 
                base(context, cancellationToken)
            {
                _frame = Time.frameCount;
                _frames = frames;
            }
            
            protected override void Execute()
            {
                if (Time.frameCount - _frame < _frames)
                {
                    return;
                }
                    
                base.Execute();
            }
        }
        
        private class TimeItem : Item
        {
            private readonly float _time;
            private readonly float _delay;
        
            public TimeItem(object context, CancellationToken cancellationToken, float delay) : 
                base(context, cancellationToken)
            {
                _time = Time.time;
                _delay = delay;
            }
        
            protected override void Execute()
            {
                if (Time.time - _time < _delay)
                {
                    return;
                }
                
                base.Execute();
            }
        }
        
        private class Queue
        {
            private readonly List<Item> _items = new();

            public Task Wait(Item item)
            {
                _items.Add(item);
                return item.Task;
            }
            
            public void Invoke()
            {
                while (_items.Count > 0)
                {
                    ImmediateItems.AddRange(_items);
                    _items.Clear();
                    
                    foreach (var item in ImmediateItems)
                    {
                        item.Invoke();
                        if (!item.Task.IsCompleted)
                        {
                            ImmediateDelayItems.Add(item);
                        }
                    }
                    
                    ImmediateItems.Clear();
                }
                
                _items.AddRange(ImmediateDelayItems);
                ImmediateDelayItems.Clear();
            }
        }
        
        private static readonly List<Item> ImmediateItems = new();
        private static readonly List<Item> ImmediateDelayItems = new();

        private readonly Queue _fixedUpdateQueue = new();
        private readonly Queue _updateQueue = new();
        private readonly Queue _lateUpdateQueue = new();
        private readonly Queue _endOfFrameQueue = new();

        private void FixedUpdate() =>
            _fixedUpdateQueue.Invoke();

        private void Update()
        {
            _updateQueue.Invoke();
            StartCoroutine(EndOfFrame());
        }
        
        private void LateUpdate() =>
            _lateUpdateQueue.Invoke();
        
        private IEnumerator EndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            _endOfFrameQueue.Invoke();
        }

        public Task WaitUntilFixedUpdate(object context, CancellationToken cancellationToken) =>
            WaitUntil(_fixedUpdateQueue, context, cancellationToken);

        public Task WaitUntilFixedUpdate(object context, CancellationToken cancellationToken, int frames) =>
            WaitUntil(_fixedUpdateQueue, context, cancellationToken, frames);

        public Task WaitUntilFixedUpdate(object context, CancellationToken cancellationToken, float time) =>
            WaitUntil(_fixedUpdateQueue, context, cancellationToken, time);

        public Task WaitUntilUpdate(object context, CancellationToken cancellationToken) =>
            WaitUntil(_updateQueue, context, cancellationToken);

        public Task WaitUntilUpdate(object context, CancellationToken cancellationToken, int frames) =>
            WaitUntil(_updateQueue, context, cancellationToken, frames);

        public Task WaitUntilUpdate(object context, CancellationToken cancellationToken, float time) =>
            WaitUntil(_updateQueue, context, cancellationToken, time);

        public Task WaitUntilLateUpdate(object context, CancellationToken cancellationToken) =>
            WaitUntil(_lateUpdateQueue, context, cancellationToken);

        public Task WaitUntilLateUpdate(object context, CancellationToken cancellationToken, int frames) =>
            WaitUntil(_lateUpdateQueue, context, cancellationToken, frames);

        public Task WaitUntilLateUpdate(object context, CancellationToken cancellationToken, float time) =>
            WaitUntil(_lateUpdateQueue, context, cancellationToken, time);

        public Task WaitUntilEndOfFrame(object context, CancellationToken cancellationToken) =>
            WaitUntil(_endOfFrameQueue, context, cancellationToken);

        public Task WaitUntilEndOfFrame(object context, CancellationToken cancellationToken, int frames) =>
            WaitUntil(_endOfFrameQueue, context, cancellationToken, frames);

        public Task WaitUntilEndOfFrame(object context, CancellationToken cancellationToken, float time) =>
            WaitUntil(_endOfFrameQueue, context, cancellationToken, time);

        private static Task WaitUntil(Queue queue, object context, CancellationToken cancellationToken)
        {
            var item = new Item(context, cancellationToken);
            return Wait(queue, item);
        }

        private static Task WaitUntil(Queue queue, object context, CancellationToken cancellationToken, int frames)
        {
            var frameItem = new FrameItem(context, cancellationToken, frames);
            return Wait(queue, frameItem);
        }

        private static Task WaitUntil(Queue queue, object context, CancellationToken cancellationToken, float time)
        {
            var timeItem = new TimeItem(context, cancellationToken, time);
            return Wait(queue, timeItem);
        }

        private static Task Wait(Queue queue, Item item)
        {
            var task = queue.Wait(item);
            return task;
        }
    }
}