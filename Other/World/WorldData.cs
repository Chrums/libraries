using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fizz6.Collections.Graph;
using Fizz6.Roguelike.World.Map;
using Fizz6.Serialization;
using Newtonsoft.Json;

namespace Fizz6.Roguelike.World
{
    public class WorldData
    {
        public class Vertex
        {
            [JsonProperty]
            public int Depth { get; set; }
            
            [JsonProperty]
            public MapConfig MapConfig { get; set; }
        }
        
        public static void Save(WorldData worldData, string path)
        {
            var serializer = new Serializer();
            var json = serializer.Serialize(worldData);
            var streamWriter = new StreamWriter(path);
            streamWriter.Write(json);
            streamWriter.Close();
        }

        public static WorldData Load(string path)
        {
            var streamReader = new StreamReader(path);
            var json = streamReader.ReadToEnd();
            streamReader.Close();
            var serializer = new Serializer();
            return serializer.Deserialize<WorldData>(json);
        }
        
        public static WorldData Generate()
        {
            var worldData = new WorldData
            {
                graph = new Graph<Vertex>()
            };

            var root = new Vertex
            {
                Depth = 0,
                MapConfig = MapConfig.Random
            };

            worldData.graph.Add(root);

            for (var depth = 1; depth < 8; ++depth)
            {
                var count = UnityEngine.Random.Range(2, 4);
                for (var index = 0; index < count; ++index)
                {
                    var vertex = new Vertex
                    {
                        Depth = depth,
                        MapConfig = MapConfig.Random
                    };

                    worldData.graph.Add(vertex);
                }
            }

            var terminal = new Vertex
            {
                Depth = 8,
                MapConfig = MapConfig.Random
            };

            worldData.graph.Add(terminal);

            for (var depth = 0; depth < 8; ++depth)
            {
                var current = worldData.Depths[depth];
                var next = worldData.Depths[depth + 1];
                foreach (var vertex0 in current)
                {
                    foreach (var vertex1 in next)
                    {
                        worldData.graph.Add(vertex0, vertex1);
                    }
                }
            }

            return worldData;
        }

        [JsonProperty]
        private Graph<Vertex> graph;
        [JsonIgnore]
        public IReadOnlyGraph<Vertex> Graph => graph;

        [JsonIgnore]
        private Dictionary<int, HashSet<Vertex>> depths;
        [JsonIgnore]
        public IReadOnlyDictionary<int, HashSet<Vertex>> Depths
        {
            get
            {
                if (depths != null) return depths;
                
                depths = graph.Vertices
                    .GroupBy(vertex => vertex.Depth)
                    .ToDictionary(
                        group => group.Key,
                        group => new HashSet<Vertex>(group.ToList())
                    );
                
                return depths;
            }
        }
    }
}