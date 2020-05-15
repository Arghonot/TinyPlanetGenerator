using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public class AddTry : MonoBehaviour
{
    public MeshRenderer Perlin;
    public MeshRenderer Voronoi;
    public MeshRenderer Mix;
    public MeshRenderer Terrace;

    public float one;
    public float two;

    public int Size;

    public int seed;
    public Texture2D TerraceTex;

    [SerializeField] Gradient _gradient;

    [SerializeField] float _left = 6;

    [SerializeField] float _right = 10;

    [SerializeField] float _top = 1;

    [SerializeField] float _bottom = 5;

    public bool Do;

    public AnimationCurve Acurve;

    public double Frequency = 4;
    public double Displacement = .1;

    private void Update()
    {
        if (Do)
        {
            Do = false;
            GenerateOwnBlend();
        }
    }

    void GenerateOwnBlend()
    {
        ModuleBase perlin = new Perlin(1, 2, .5, 6, 42, QualityMode.Medium);
        ModuleBase voronoi = new Voronoi(Frequency, Displacement, 42, true);
        //ModuleBase blend = new Add(perlin, voronoi);
        Curve curve = new Curve(perlin);

        foreach (var point in Acurve.keys)
        {
            curve.Add(point.time, point.value);
        }

        //curve.Add(0d, .1d);
        //curve.Add(.5d, .5d);
        //curve.Add(1.9d, .9d);

        var perlinbuilder = new Noise2D(Size, Size / 2, perlin);
        perlinbuilder.GeneratePlanar(_left, _right, _top, _bottom);

        var voronoibuilder = new Noise2D(Size, Size / 2, voronoi);
        voronoibuilder.GeneratePlanar(_left, _right, _top, _bottom);

        var blendbuilder = new Noise2D(Size, Size / 2, curve);
        blendbuilder.GeneratePlanar(_left, _right, _top, _bottom);

        Perlin.material.SetTexture("_BaseMap", perlinbuilder.GetTexture(_gradient));
        Voronoi.material.SetTexture("_BaseMap", voronoibuilder.GetTexture(_gradient));
        Mix.material.SetTexture("_BaseMap", blendbuilder.GetTexture(_gradient));
    }

    void GenerateOwnTests()
    {
        ModuleBase perlin = new Perlin(1, 2, .5, 6, seed, QualityMode.Medium);
        ModuleBase voronoi = new Voronoi(Frequency, Displacement, seed, true);
        ModuleBase add = new Add(perlin, voronoi);

        Terrace terrace = new Terrace(false, add);
        terrace.Add(0f);
        terrace.Add(one);
        terrace.Add(two);

        var perlinbuilder = new Noise2D(Size, Size / 2, perlin);
        perlinbuilder.GeneratePlanar(_left, _right, _top, _bottom);

        var voronoibuilder = new Noise2D(Size, Size / 2, voronoi);
        voronoibuilder.GeneratePlanar(_left, _right, _top, _bottom);

        var addbuilder = new Noise2D(Size, Size / 2, add);
        addbuilder.GeneratePlanar(_left, _right, _top, _bottom);

        var terracebuilder = new Noise2D(Size, Size / 2, terrace);
        terracebuilder.GeneratePlanar(_left, _right, _top, _bottom);

        var perlintex = perlinbuilder.GetTexture(_gradient);
        var voronoitex = voronoibuilder.GetTexture(_gradient);
        var addtex = addbuilder.GetTexture(_gradient);
        var terracetex = terracebuilder.GetTexture(_gradient);

        perlintex.Apply();
        voronoitex.Apply();
        addtex.Apply();
        terracetex.Apply();

        Perlin.material.SetTexture("_BaseMap", perlintex);
        Voronoi.material.SetTexture("_BaseMap", voronoitex);
        Mix.material.SetTexture("_BaseMap", addtex);
        Terrace.material.SetTexture("_BaseMap", terracetex);

        //Perlin.material.SetTexture("_BaseMap", perlinbuilder.GetTexture(_gradient));
        //Voronoi.material.SetTexture("_BaseMap", voronoibuilder.GetTexture(_gradient));
        //Mix.material.SetTexture("_BaseMap", addbuilder.GetTexture(_gradient));
        //Terrace.material.SetTexture("_BaseMap", terracebuilder.GetTexture(_gradient));
    }
}
