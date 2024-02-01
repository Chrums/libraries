using UnityEngine;

namespace Fizz6.Roguelike.World.Map.Spawner
{
    public abstract class Spawner<T> : MonoBehaviour
        where T : class
    {
        [SerializeField, Range(0.0f, 1.0f)]
        private float probability = 0.05f;
        
        protected Map Map { get; private set; }

        public T Invoke(Map map)
        {
            Map = map;
            var random = Random.Range(0, 1.0f);
            return random < probability
                ? Spawn()
                : null;
        }

        protected abstract T Spawn();
    }
}