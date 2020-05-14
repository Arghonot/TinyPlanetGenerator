using LibNoise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGraphRunner : Graph.GraphRunner<NoiseGraph.LibnoiseGraph>
{
    public bool Gen;
    public float south = 90.0f;
    public float north = -90.0f;
    public float west = -180.0f;
    public float east = 180.0f;
    public int size;
    public Texture2D ColorMap;

    private void Update()
    {
        if (Gen)
        {
            Generate();
            Gen = false;
        }
    }

    void Generate()
    {
        var generator = graph.GetGenerator();

        Noise2D map = new Noise2D(size, size / 2, generator);

        map.GenerateSpherical(
            south,
            north,
            west,
            east);

        ColorMap = map.GetTexture();
        ColorMap.Apply();
    }

    //public NoiseGraph.LibnoiseGraph graph;

    //public bool Gen;



    //private void Update()
    //{
    //    if (Gen)
    //    {
    //        Generate();
    //        Gen = false;
    //    }
    //}

    //void Generate()
    //{
    //    var generator = graph.GetGenerator();

    //    Noise2D map = new Noise2D(size, size / 2, generator);

    //    map.GenerateSpherical(
    //        south,
    //        north,
    //        west,
    //        east);

    //    ColorMap = map.GetTexture();
    //    ColorMap.Apply();
    //}
}
