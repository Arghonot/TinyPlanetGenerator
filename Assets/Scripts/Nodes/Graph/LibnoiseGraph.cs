using Graph;
using System.Linq;
using UnityEngine;

namespace NoiseGraph
{
    [CreateAssetMenu]
    public class LibnoiseGraph : DefaultGraph
    {
        public RootModuleBase Root;

        public SerializableModuleBase GetGenerator(GenericDicionnary newgd = null)
        {
            if (newgd != null)
            {
                this.gd = newgd;
            }

            return (SerializableModuleBase)Root.GetValue(Root.Ports.First());
        }
    }
}