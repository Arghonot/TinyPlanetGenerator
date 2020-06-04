using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Transformer/Translate")]
    public class TranslateNode : LibnoiseNode
    {

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double X;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Y;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Z;

        public override object Run()
        {
            return new Translate(
                GetInputValue<double>("X", this.X),
                GetInputValue<double>("Y", this.Y),
                GetInputValue<double>("Z", this.Z),
                GetInputValue<SerializableModuleBase>("Input", this.Input));
        }
    }
}