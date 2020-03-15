using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlanetType
{
    Sun,
    TelluricSimple,
    Gaz,
    Magma,
    water,
    MoonLike
}

[CreateAssetMenu(fileName = "Profile", menuName = "Planets/Profile", order = 1)]
public class PlanetProfile : ScriptableObject
{
    public string Name;
    public Gradient ColorMap;
    public Gradient ElevationMap;
    public Gradient InverseElevationMap;
    public float ElevationMultiplier;
    public float BaseElevation = .35f;
    public NoiseType type;

    public Material material;

    // Noise relative
    public double frequency = 1d;
    public double lacunarity = 2d;
    public double persistence = .5d;
    public double displacement = 0d;
    public int octaves = 6;
}
