using LibNoise;
using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Generator/Checker")]
    public class CheckerNode : Graph.Branch
    {
        [Output(ShowBackingValue.Always, ConnectionType.Multiple, TypeConstraint.Strict)]
        public ModuleBase GeneratorOutput;

        public void Awake()
        {
            AddDynamicOutput(
                typeof(SerializableModuleBase),
                ConnectionType.Multiple,
                TypeConstraint.Strict,
                "");
        }

        public override object Run()
        {
            return new Checker();
        }
    }
}