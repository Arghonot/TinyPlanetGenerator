using LibNoise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NoiseGraph
{
    public class RootModuleBase : Graph.Root<SerializableModuleBase>
    {
        public override object Run()
        {
            return GetInputValue("Input", new SerializableModuleBase(0));
        }
    }
}