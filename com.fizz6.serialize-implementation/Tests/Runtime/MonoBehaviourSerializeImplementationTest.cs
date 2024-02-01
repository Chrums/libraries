using System;
using UnityEngine;

namespace Fizz6.SerializeImplementation.Tests
{
    [Serializable]
    public abstract class MonoBehaviourSerializeImplementationTest : MonoBehaviourSerializeImplementation
    {}
    
    [Serializable]
    public class SerializeImplementationMonoBehaviourTimeTest : MonoBehaviourSerializeImplementationTest
    {
        public override void Update()
        {
            base.Update();
            
            Debug.Log(Time.time);
        }
    }
    
    [Serializable]
    public class SerializeImplementationMonoBehaviourFrameTest : MonoBehaviourSerializeImplementationTest
    {
        public override void Update()
        {
            base.Update();
            
            Debug.Log(Time.frameCount);
        }
    }
}