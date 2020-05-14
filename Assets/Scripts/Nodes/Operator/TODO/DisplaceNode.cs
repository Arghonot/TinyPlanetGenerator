using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Operator/Displace")]
    public class DisplaceNode : LibnoiseNode
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Source;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase ControllerA;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase ControllerB;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase ControllerC;

        public override object Run()
        {
            return new Displace(
                GetInputValue<SerializableModuleBase>("Source", this.Source),
                GetInputValue<SerializableModuleBase>("ControllerA", this.ControllerA),
                GetInputValue<SerializableModuleBase>("ControllerB", this.ControllerB),
                GetInputValue<SerializableModuleBase>("ControllerC", this.ControllerC));
        }
    }
}