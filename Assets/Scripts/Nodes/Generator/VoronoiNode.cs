using LibNoise;
using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Generator/Voronoi")]
    public class VoronoiNode : Graph.Branch
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double frequency;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double displacement;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public int Seed;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public bool distance;

        [Output(ShowBackingValue.Always, ConnectionType.Multiple, TypeConstraint.Strict)]
        public ModuleBase GeneratorOutput;

        public void Awake()
        {
            AddDynamicOutput(
                typeof(SerializableModuleBase),
                ConnectionType.Multiple,
                TypeConstraint.Strict,
                "Output");
        }

        public override object Run()
        {
            // if editing the graph -> we stick to current variables
            if (Application.isEditor && !Application.isPlaying)
            {
                return new Voronoi(
                    this.frequency,
                    this.displacement,
                    this.Seed,
                    this.distance);
            }

            //return new Voronoi(frequency, displacement, Seed, distance);
            return new Voronoi(
                GetInputValue<double>("frequency", this.frequency),
                GetInputValue<double>("displacement", this.displacement),
                GetInputValue<int>("Seed", this.Seed),
                GetInputValue<bool>("distance", this.distance));
        }
    }
}