using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Operator/Invert")]
    public class InvertNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        public override object Run()
        {
            return new Invert(
                GetInputValue<SerializableModuleBase>("Input", this.Input));

        }
    }
}