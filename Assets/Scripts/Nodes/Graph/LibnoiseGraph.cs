using Graph;
using System.Linq;
using UnityEngine;

namespace NoiseGraph
{
    [CreateAssetMenu]
    public class LibnoiseGraph : DefaultGraph
    {
        public RootModuleBase Root;

        public SerializableModuleBase GetGenerator()
        {
            return (SerializableModuleBase)Root.GetValue(Root.Ports.First());
        }
    }
}