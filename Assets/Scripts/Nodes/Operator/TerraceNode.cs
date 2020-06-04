using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Modifier/Terrace")]
    public class TerraceNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        [Input(dynamicPortList = true)]
        public List<double> Terrace = new List<double>();

        public override object Run()
        {
            Terrace terr = new Terrace(
                GetInputValue<SerializableModuleBase>("Input", this.Input));

            for (int i = 0; i < Terrace.Count; i++)
            {
                terr.Add
                    (GetInputValue<double>(
                        "Terrace " + i.ToString(),
                        this.Terrace[i]));
            }

            return terr;
        }
    }
}