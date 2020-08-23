using LibNoise;
using LibNoise.Generator;
using UnityEngine;
using XNode;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Generator/Perlin")]
    public class PerlinNode : Graph.Branch
    {
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double frequency;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double lacunarity;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public double persistence;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public int Octaves;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public int Seed;
        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public QualityMode Quality;

        //[Output(ShowBackingValue.Always, ConnectionType.Multiple, TypeConstraint.Strict)]
        //public SerializableModuleBase Generator;

        public void Awake()
        {
            AddDynamicOutput(
                typeof(SerializableModuleBase),
                ConnectionType.Multiple,
                TypeConstraint.Strict,
                "Output");
        }

        //public override object GetValue(NodePort port)
        //{
        //    return Run();
        //}

        public override object Run()
        {
            // if editing the graph -> we stick to current variables
            if (Application.isEditor && !Application.isPlaying)
            {
                return new Perlin(
                    this.frequency,
                    this.lacunarity,
                    this.persistence,
                    this.Octaves,
                    this.Seed,
                    this.Quality);
            }

            return new Perlin(
                GetInputValue<double>("frequency", this.frequency),
                GetInputValue<double>("lacunarity", this.lacunarity),
                GetInputValue<double>("persistence", this.persistence),
                GetInputValue<int>("Octaves", this.Octaves),
                GetInputValue<int>("Seed", this.Seed),
                GetInputValue<QualityMode>("Quality", this.Quality));
        }
    }
}
