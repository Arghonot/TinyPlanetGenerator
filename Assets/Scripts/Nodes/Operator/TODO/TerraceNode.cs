using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Operator/Terrace")]
    public class TerraceNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Terrace1;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Terrace2;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double Terrace3;


        public override object Run()
        {
            Terrace terr = new Terrace(
                GetInputValue<SerializableModuleBase>("Input", this.Input));

            terr.Add(GetInputValue<double>("Terrace1", this.Terrace1));
            terr.Add(GetInputValue<double>("Terrace2", this.Terrace2));
            terr.Add(GetInputValue<double>("Terrace3", this.Terrace3));

            return terr;
        }
    }
}