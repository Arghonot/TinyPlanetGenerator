using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [System.Serializable]
    [CreateNodeMenu("NoiseGraph/Operator/Curve")]
    public class CurveNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        public AnimationCurve InputCurve;

        public override object Run()
        {
            Curve curve = new Curve(
                GetInputValue<SerializableModuleBase>("Input", this.Input));

            foreach (var point in InputCurve.keys)
            {
                curve.Add(point.time, point.value);
            }

            return curve;
        }
    }
}