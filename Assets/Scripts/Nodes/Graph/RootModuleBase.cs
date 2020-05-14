
using System;

namespace NoiseGraph
{
    [Serializable]
    public class RootModuleBase : Graph.Root<SerializableModuleBase>
    {
        public override object Run()
        {
            return GetInputValue("Input", new SerializableModuleBase(0));
        }
    }
}