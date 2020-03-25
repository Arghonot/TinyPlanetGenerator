using UnityEngine;

using LibNoise.Unity;
using LibNoise.Unity.Operator;
using LibNoise.Unity.Generator;

public class CustomPerlinGenerator : MonoBehaviour
{
    public float turbulence;
    public int mapSize;

    public float south = -90.0f;
    public float north = 90.0f;

    public float west = -180.0f;
    public float east = 180.0f;

    public Texture2D ColorMap;
    public Texture2D HeightMap;
    public Texture2D InverseHeightMap;

    public void Generate(PlanetProfile profile)
    {
        ModuleBase Generator;

        switch (profile.type)
        {

            case NoiseType.Perlin:
                Generator = new Perlin(
                    profile.frequency,
                    profile.lacunarity,
                    profile.persistence,
                    profile.octaves,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
            case NoiseType.Billow:
                Generator = new Billow(
                    profile.frequency,
                    profile.lacunarity,
                    profile.persistence,
                    profile.octaves,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
            case NoiseType.RiggedMultifractal:
                Generator = new RiggedMultifractal(profile.frequency,
                    profile.lacunarity,
                    profile.octaves,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
            case NoiseType.Voronoi:
                Generator = new Voronoi(
                    profile.frequency,
                    profile.displacement,
                    Random.Range(0, int.MaxValue),
                    true);

                break;
            default:
                Generator = new Perlin(
                    profile.frequency,
                    profile.lacunarity,
                    profile.persistence,
                    profile.octaves,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
        }

        //Generator2 = new Perlin(1, 2, .5, 6, 10, QualityMode.High);
        //Generator2 = new Voronoi(1, 2, 10, false);
        //Generator2 = new RiggedMultifractal(1, 2, 6, 10, QualityMode.High);
        //Generator2 = new Billow();
        //Generator3 = new RiggedMultifractal();

        //Module1 = new ScaleBias(0.125, -0.75, Generator);
        //Module2 = new Select(0, 1, 0.125, Module1, Generator3);
        //Module2 = new Select(Generator, Generator2, Generator3);

        //Module3 = new Turbulence()
        //Add add = new Add(Generator, Generator2);

        Noise2D map = new Noise2D(mapSize, mapSize / 2, Generator);


        map.GenerateSpherical(
            south,
            north,
            west,
            east);

        ColorMap = map.GetTexture(profile.ColorMap);
        ColorMap.Apply();

        HeightMap = map.GetTexture(profile.ElevationMap);
        HeightMap.Apply();

        //InverseHeightMap = map.GetTexture(profile.InverseElevationMap);
        //InverseHeightMap.Apply();

        UIMapManager.Instance.AddMap(ColorMap, string.Join(" ", new string[]
        {
            profile.Name,
            "Colormap"
        }));
        UIMapManager.Instance.AddMap(HeightMap, string.Join(" ", new string[]
        {
            profile.Name,
            "HeightMap"
        }));
        //UIMapManager.Instance.AddMap(InverseHeightMap, string.Join(" ", new string[]
        //{
        //    profile.Name,
        //    "InverseHeightMap"
        //}));
    }
}