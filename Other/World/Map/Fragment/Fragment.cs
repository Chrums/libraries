using System.Collections.Generic;
using System.Linq;
using Fizz6.Autofill;
using Fizz6.Roguelike.Minion.Abilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Fizz6.Roguelike.World.Map.Fragment
{
    public class Fragment : MonoBehaviour
    {
        public static readonly Vector3Int Dimensions = new(32, 32, 0);
        
        [SerializeField, Autofill] 
        private Tilemap tilemap;
        public Tilemap Tilemap => tilemap;

        [SerializeField] 
        private FragmentConnection connections;
        public FragmentConnection Connections
        {
            get => connections;
            set => connections = value;
        }

        public void OnDrawGizmos()
        {
            var size = new Vector3Int(Dimensions.x, Dimensions.y, 0);
            Gizmos.DrawWireCube(gameObject.transform.position, size);
            if (!connections.HasFlag(FragmentConnection.North))
                Gizmos.DrawRay(transform.position + Vector3.up * 0.5f + Vector3.left * 0.5f, Vector3.right);
            if (!connections.HasFlag(FragmentConnection.East))
                Gizmos.DrawRay(transform.position + Vector3.right * 0.5f + Vector3.up * 0.5f, Vector3.down);
            if (!connections.HasFlag(FragmentConnection.South))
                Gizmos.DrawRay(transform.position + Vector3.down * 0.5f + Vector3.right * 0.5f, Vector3.left);
            if (!connections.HasFlag(FragmentConnection.West))
                Gizmos.DrawRay(transform.position + Vector3.left * 0.5f + Vector3.down * 0.5f, Vector3.up);
        }
    }
}