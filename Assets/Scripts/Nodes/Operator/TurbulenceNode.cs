using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Transformer/Turbulence")]
    public class TurbulenceNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Power;

        public override object Run()
        {
            Debug.Log("Run turbulence " + GetInputValue<double>("Power", this.Power));
            return new Turbulence(
                GetInputValue<double>("Power", this.Power),
                GetInputValue<SerializableModuleBase>("Input", this.Input));
        }
    }
}