using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

public class NoiseValues
{
    public int toto;
}

[CreateAssetMenu(fileName = "Profile", menuName = "Planets/Profile", order = 1)]
public class PlanetProfile : ScriptableObject
{
    public string Name;

    public int TexturesSize = 512;
    public Gradient ColorGradient;
    public Gradient ElevationGradient;

    public float ElevationMultiplier;
    public float BaseElevation = .35f;
    public NoiseType type;

    public NoiseGraph.LibnoiseGraph graph;

    // Noise relative
    public double frequency = 1d;
    public double lacunarity = 2d;
    public double persistence = 0.5d;
    public double displacement = 0d;
    public int octaves = 6;
    public bool distance = true;

    public Material material;
    public Material WaterMaterial;
    public Material AuraMaterial;

    public Color SunFresnelColor;

    public bool UseGround = true;
    public bool UseWater = false;
    public bool UseClouds = false;
    public bool UseAura = true;

    public float SeaLevel = 1f;
    public Color WaterColor = new Color(44, 78, 79);
    public Color RippleColor = new Color(4, 18, 20);

    [Range(0f, 1f)]
    public float CliffIntensity = 0.1f;
    public Color CliffLightColor = new Color(152, 11, 127);
    public Color CliffDarkColor = new Color(142, 104, 117);

    public float AuraIntensity = 7.73f;
    public Color Aura = Color.blue;

    // TODO work on some getter and setters to make it private later on
    public Graph.SerializableBlackBoard sbb;

    public SerializableModuleBase Run()
    {
        return graph.GetGenerator(GetArguments());
    }

    public void OnDragGraph()
    {

    }

    // TODO build GD only based on noise attr
    Graph.GenericDicionnary GetArguments()
    {
        Graph.GenericDicionnary gd = new Graph.GenericDicionnary();

        foreach (var item in this.GetType().GetFields())
        {
            gd.Add(item.Name, item.GetValue(this));
        }

        return gd;
    }

    private void Awake()
    {
        if (sbb == null) sbb = new Graph.SerializableBlackBoard();
    }
}
