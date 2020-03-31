﻿using System.Collections;
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

    public int TexturesSize = 512;
    public Gradient ColorMap;
    public Gradient ElevationMap;

    public float ElevationMultiplier;
    public float BaseElevation = .35f;
    public NoiseType type;

    // Noise relative
    public double frequency = 1d;
    public double lacunarity = 2d;
    public double persistence = .5d;
    public double displacement = 0d;
    public int octaves = 6;

    public Material material;

    public bool useWater = false;
    public bool UseClouds = false;
    public bool UseAura = true;

    public Color WaterColor = new Color(44, 78, 79);
    public Color RippleColor = new Color(4, 18, 20);

    public float CliffIntensity = 0.1f;
    public Color CliffLightColor = new Color(152, 11, 127);
    public Color CliffDarkColor = new Color(142, 104, 117);

    public float AuraIntensity = 7.73f;
    public Color Aura = Color.blue;
}
