using LibNoise;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NoiseGraph
{
    [CreateNodeMenu("NoiseGraph/Debug/Render")]
    [NodeTint(Graph.ColorProfile.Debug)]
    public class Renderer : Node
    {
        float south = 90.0f;
        float north = -90.0f;
        float west = -180.0f;
        float east = 180.0f;
        public int size = 512;

        public float Space = 110;

        [Input(ShowBackingValue.Always, ConnectionType.Override, TypeConstraint.Strict)]
        public SerializableModuleBase Input;

        public Rect TexturePosition = new Rect(14, 130, 180, 90);
        public Texture2D tex = null;
        public Gradient grad;

        public long RenderTime;

        public void Render()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            Noise2D map = new Noise2D(
                size,
                size / 2, 
                GetInputValue<SerializableModuleBase>("Input", this.Input));

            map.GenerateSpherical(
                south,
                north,
                west,
                east);

            tex = map.GetTexture(grad);
            tex.Apply();

            watch.Stop();
            RenderTime = watch.ElapsedMilliseconds;
        }
    }
}    