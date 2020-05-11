using Graph;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace NoiseGraph
{
    [CreateAssetMenu]
    public class LibnoiseGraph : DefaultGraph
    {
        public RootModuleBase Root;

        private new void Awake()
        {
            base.Awake();

            Root = AddNode<RootModuleBase>();

            Root.name = "Root";
        }

        public SerializableModuleBase GetGenerator()
        {
            return (SerializableModuleBase)Root.GetValue(Root.Ports.First());
            //return Root.GetInputValue<SerializableModuleBase>("Input");
        }

    }
}