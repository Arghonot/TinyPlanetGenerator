using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    /// <summary>
    /// A default class with a generic output node.
    /// </summary>
    public class LibnoiseNode : Graph.Branch
    {
        private void Awake()
        {
            AddDynamicOutput(
                typeof(SerializableModuleBase),
                ConnectionType.Multiple,
                TypeConstraint.Strict,
                "");
        }

        public override object Run()
        {
            return null;
        }
    }
}