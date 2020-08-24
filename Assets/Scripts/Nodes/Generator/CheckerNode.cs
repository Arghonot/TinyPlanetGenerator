using LibNoise;
using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Generator/Checker")]
    public class CheckerNode : LibnoiseNode
    {
        [Output(ShowBackingValue.Always, ConnectionType.Multiple, TypeConstraint.Strict)]
        public ModuleBase GeneratorOutput;

        public override object Run()
        {
            return new Checker();
        }
    }
}