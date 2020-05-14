using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static XNode.Node;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Operator/Clamp")]
    public class ClampNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Minimum;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Maximum;

        public override object Run()
        {
            Clamp clamp = new Clamp(
                GetInputValue<SerializableModuleBase>(
                    "Input",
                    this.Input));

            clamp.Maximum =
                GetInputValue<double>(
                    "Minimum",
                    this.Minimum);
            clamp.Maximum =
                GetInputValue<double>(
                    "Maximum",
                    this.Maximum);

            return clamp;
        }
    }
}