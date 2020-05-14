using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    public enum Quality
    {
        Low,
        Medium,
        High
    }

    [CreateNodeMenu("NoiseGraph/Input/Quality")]
    public class QualityNode : Graph.Leaf
    {
        [Output(ShowBackingValue.Always, ConnectionType.Multiple, TypeConstraint.Strict)]
        public Quality Quality;

        public override object Run()
        {
            return Quality;
        }
    }
}
