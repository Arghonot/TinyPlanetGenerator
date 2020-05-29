using UnityEngine;

using LibNoise;
using LibNoise.Operator;
using LibNoise.Generator;
using System.Collections.Generic;

public enum NoiseType
{
    Perlin,
    Billow,
    RiggedMultifractal,
    Voronoi
}

public class CustomPerlinGenerator : Singleton<CustomPerlinGenerator>
{
    public float turbulence;
    public int mapSize;

    public float south = 90.0f;
    public float north = -90.0f;

    public float west = -180.0f;
    public float east = 180.0f;

    public Texture2D ColorMap;
    public Texture2D HeightMap;
    public Texture2D InverseHeightMap;
    public List<Texture2D> imgs;

    public void Generate(PlanetGenerationData datas)
    {
        mapSize = datas.mapSize;

        Generate(datas.profile);
        // End
        datas.Callback();
    }

    public void Generate(PlanetProfile profile)
    {
        //mapSize = 32;
        imgs = new List<Texture2D>();
        List<ModuleBase> generators = new List<ModuleBase>();

        generators.Add(GetModule(profile));

        // TODO USE A HEIGHTMAP THAT HAS THE SAME AMOUNT OF PXL THAN VERTICES
        // e.g : 80 * 80
        // if the map is 512 -> we get to have 6.4 planet's heightmap for the cost of a single map
        Noise2D map = new Noise2D(mapSize, mapSize / 2, generators[0]);

        map.GenerateSpherical(
            south,
            north,
            west,
            east);

        ColorMap = map.GetTexture(profile.ColorMap);
        ColorMap.Apply();

        HeightMap = map.GetTexture(profile.ElevationMap);
        HeightMap.Apply();
    }

    public Texture2D GetCloudBase(UnityEngine.Gradient cloudGradient, int size, NoiseType type)
    {
        ModuleBase Generator = GetModule(type);
        //Billow Generator = new Billow(
        //    1f,
        //    2f,
        //    0.5f,
        //    6,
        //    Random.Range(0, int.MaxValue),
        //    QualityMode.Low);

        Noise2D map = new Noise2D(size, size / 2, Generator);

        map.GenerateSpherical(
            south,
            north,
            west,
            east);

        var tex = map.GetTexture(cloudGradient);
        tex.Apply();

        return tex;
    }

    ModuleBase GetModule(PlanetProfile profile)
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
                Generator = new RidgedMultifractal(profile.frequency,
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

        return Generator;
    }

    ModuleBase GetModule(NoiseType type)
    {
        ModuleBase Generator;

        switch (type)
        {

            case NoiseType.Perlin:
                Generator = new Perlin(
                    1d,
                    2d,
                    .5d,
                    6,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
            case NoiseType.Billow:
                Generator = new Billow(
                    1d,
                    2d,
                    .5d,
                    6,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
            case NoiseType.RiggedMultifractal:
                Generator = new RidgedMultifractal(
                    1d,
                    2d,
                    6,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
            case NoiseType.Voronoi:
                Generator = new Voronoi(
                    1d,
                    0,
                    Random.Range(0, int.MaxValue),
                    true);

                break;
            default:
                Generator = new Perlin(
                    1d,
                    2d,
                    .5d,
                    6,
                    Random.Range(0, int.MaxValue),
                    QualityMode.Low);

                break;
        }

        return Generator;
    }
}