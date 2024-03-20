using System.Threading;
using UnityEngine;

namespace Fizz6.Scheduler.Tests
{
    public class SchedulerTestInvoker : MonoBehaviour
    {
        private async void Awake()
        {
            var cancellationToken = new CancellationToken();
            await this.WaitUntilUpdate(2.0f, cancellationToken);
            Debug.Log("Invoke In Update After 2 Seconds");
            InvokeInLateUpdateTest();
            await this.WaitUntilUpdate(1.0f);
            Debug.Log("Invoke In Update After 1 Second");
        }

        private async void InvokeInLateUpdateTest()
        {
            await this.WaitUntilLateUpdate(1.0f);
            Debug.Log("Invoke In Late Update After 1 Second");
        }
    }
}