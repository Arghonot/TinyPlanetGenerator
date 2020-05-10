using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

/// <summary>
/// See http://libnoise.sourceforge.net/tutorials/tutorial5.html for an explanation
/// </summary>
public class Tutorial5 : MonoBehaviour
{
    public int seed;
    public MeshRenderer Step1;
    public MeshRenderer Step2;
    public MeshRenderer Step3;
    public MeshRenderer Step4;

    [SerializeField] Gradient _gradient;

    [SerializeField] float _left = 6;

    [SerializeField] float _right = 10;

    [SerializeField] float _top = 1;

    [SerializeField] float _bottom = 5;

    [SerializeField] int _tutorialStep = 1;


    public float south = 90.0f;
    public float north = -90.0f;

    public float west = -180.0f;
    public float east = 180.0f;

    public bool Do;

    private void Update()
    {
        if (Do)
        {
            Do = false;
            Generate();
        }
    }

    void Generate()
    {
        // STEP 1
        // Gradient is set directly on the object
        var mountainTerrain = new RidgedMultifractal();
        RenderAndSetImage(mountainTerrain, Step1);

        // Stop rendering if we're only getting as far as this tutorial
        // step. It saves me from doing multiple files.
        if (_tutorialStep <= 1) return;

        // STEP 2
        var baseFlatTerrain = new Billow();
        baseFlatTerrain.Frequency = 0.1;
        RenderAndSetImage(baseFlatTerrain, Step2);


        if (_tutorialStep <= 2) return;

        // STEP 3
        var flatTerrain = new ScaleBias(0.125, -0.75, baseFlatTerrain);
        RenderAndSetImage(flatTerrain, Step3);

        if (_tutorialStep <= 3) return;

        // STEP 4
        var terrainType = new Perlin();
        terrainType.Frequency = .5;
        terrainType.Persistence = 0.25;
        var finalTerrain = new Select(baseFlatTerrain, mountainTerrain, terrainType);
        finalTerrain.SetBounds(0, 1000);
        finalTerrain.FallOff = 0.125;
        RenderAndSetImage(finalTerrain, Step4);
    }

    void RenderAndSetImage(ModuleBase generator, MeshRenderer rend)
    {
        var heightMapBuilder = new Noise2D(256, 256, generator);
        heightMapBuilder.GeneratePlanar(_left, _right, _top, _bottom);

        //heightMapBuilder.GenerateSpherical(
        //    south,
        //    north,
        //    west,
        //    east);

        var image = heightMapBuilder.GetTexture(_gradient);
        rend.material.SetTexture("_BaseMap", image);
    }

}