using UnityEngine.UI;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using System.Globalization;

using TMPro;
using System.Runtime.InteropServices;

public enum MapType
{
    Planar,
    Spherical,
    Cylindrical
}

public enum DisplayShape
{
    Plane,
    Sphere,
    Cube,
    Cylinder
}

/// <summary>
/// This is designed to be a very simple tool to allow the quick creation of noise maps for direct use.
/// </summary>
public class NoiseTool : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ImageDownloader(string str, string fn);

    public MeshRenderer Plane;
    public MeshRenderer Cube;
    public MeshRenderer Sphere;
    public MeshRenderer Cylinder;

    public Material CommonMaterial;

    public NoiseType currentType;
    public MapType currentMapForm;
    public DisplayShape CurrentDisplayShape;

    public Texture2D tex;

    #region UI

    public TMP_Dropdown Type;
    public TMP_Dropdown Form;
    public TMP_Dropdown Shape;

    public InputField Input_Frequency;
    public InputField Input_Lacunarity;
    public InputField Input_Persistence;
    public InputField Input_Displacement;
    public InputField Input_Octave;
    public InputField Input_Seed;
    public InputField Input_Width;
    public InputField Input_Height;

    public Button Button_Generate;
    public Button Button_Download;

    #endregion

    /// <summary>
    /// Yes -> fit a sphere
    /// No -> fit a flat / cubic object
    /// </summary>
    public bool Spherical;

    // Noise relative
    public double frequency = 1d;
    public double lacunarity = 2d;
    public double persistence = .5d;
    public double displacement = 0d;
    public int octaves = 6;
    public int seed = 42;
    public bool Distance = true;
    public int Width = 512;
    public int Height = 256;

    #region Coords
    float south = -90.0f;
    float north = 90.0f;

    float west = -180.0f;
    float east = 180.0f;
    #endregion

    private void Start()
    {
        InitializeUI();
        InitializeScene();
    }

    void InitializeScene()
    {
        // we make them share the same material
        Plane.sharedMaterial = CommonMaterial;
        Sphere.sharedMaterial = CommonMaterial;
        Cube.sharedMaterial = CommonMaterial;
        Cylinder.sharedMaterial = CommonMaterial;

        SetDisplayShape();
    }

    private void SetDisplayShape()
    {
        Plane.gameObject.SetActive(
            CurrentDisplayShape == DisplayShape.Plane ? true : false);
        Cube.gameObject.SetActive(
            CurrentDisplayShape == DisplayShape.Cube ? true : false);
        Sphere.gameObject.SetActive(
            CurrentDisplayShape == DisplayShape.Sphere ? true : false);
        Cylinder.gameObject.SetActive(
            CurrentDisplayShape == DisplayShape.Cylinder ? true : false);
    }

    #region UI Functions

    void InitializeUI()
    {
        Input_Frequency.text = frequency.ToString();
        Input_Lacunarity.text = lacunarity.ToString();
        Input_Persistence.text = persistence.ToString();
        Input_Displacement.text = displacement.ToString();
        Input_Octave.text = octaves.ToString();
        Input_Seed.text = seed.ToString();
        Input_Width.text = Width.ToString();
        Input_Height.text = Height.ToString();
    }

    public void UpdateType()
    {
        switch (Type.value)
        {
            case 0: // Perlin
                currentType = NoiseType.Perlin;

                Input_Frequency.gameObject.SetActive(true);
                Input_Lacunarity.gameObject.SetActive(true);
                Input_Persistence.gameObject.SetActive(true);
                Input_Displacement.gameObject.SetActive(false);
                Input_Octave.gameObject.SetActive(true);
                Input_Seed.gameObject.SetActive(true);
                Input_Width.gameObject.SetActive(true);
                Input_Height.gameObject.SetActive(true);

                break;
            case 1: // Billow
                currentType = NoiseType.Billow;

                Input_Frequency.gameObject.SetActive(true);
                Input_Lacunarity.gameObject.SetActive(true);
                Input_Persistence.gameObject.SetActive(true);
                Input_Displacement.gameObject.SetActive(false);
                Input_Octave.gameObject.SetActive(true);
                Input_Seed.gameObject.SetActive(true);
                Input_Width.gameObject.SetActive(true);
                Input_Height.gameObject.SetActive(true);

                break;
            case 2: // Rigged multifractal
                currentType = NoiseType.RiggedMultifractal;
                Input_Frequency.gameObject.SetActive(true);
                Input_Lacunarity.gameObject.SetActive(true);
                Input_Persistence.gameObject.SetActive(false);
                Input_Displacement.gameObject.SetActive(false);
                Input_Octave.gameObject.SetActive(true);
                Input_Seed.gameObject.SetActive(true);
                Input_Width.gameObject.SetActive(true);
                Input_Height.gameObject.SetActive(true);

                break;
            case 3: // Voronoi
                currentType = NoiseType.Voronoi;

                Input_Frequency.gameObject.SetActive(true);
                Input_Lacunarity.gameObject.SetActive(false);
                Input_Persistence.gameObject.SetActive(false);
                Input_Displacement.gameObject.SetActive(true);
                Input_Octave.gameObject.SetActive(false);
                Input_Seed.gameObject.SetActive(true);
                Input_Width.gameObject.SetActive(true);
                Input_Height.gameObject.SetActive(true);

                break;
        }
    }

    public void UpdateForm()
    {
        switch (Form.value)
        {
            case 0: // Flat
                currentMapForm = MapType.Planar;
                break;
            case 1: // Spherical
                currentMapForm = MapType.Spherical;
                break;
            case 2: // Cylindric
                currentMapForm = MapType.Cylindrical;
                break;
        }
    }

    public void UpdateShape()
    {
        switch (Shape.value)
        {
            case 0: // Plane
                CurrentDisplayShape = DisplayShape.Plane;
                break;
            case 1: // Cube
                CurrentDisplayShape = DisplayShape.Cube;
                break;
            case 2: // Sphere
                CurrentDisplayShape = DisplayShape.Sphere;
                break;
            case 3: // Cylinder
                CurrentDisplayShape = DisplayShape.Cylinder;
                break;
        }

        SetDisplayShape();
    }

    public void UpdateFrequency()
    {
        frequency = ParseDouble(Input_Frequency.text, 0d);
    }

    public void UpdateLacunarity()
    {
        lacunarity = ParseDouble(Input_Lacunarity.text, 0d);
    }

    public void UpdatePersistence()
    {
        persistence = ParseDouble(Input_Persistence.text, 0d);
    }

    public void UpdateDisplacement()
    {
        displacement = ParseDouble(Input_Displacement.text, 0d);
    }

    public void UpdateOctave()
    {
        octaves = ParseInt(Input_Octave.text, 0);
    }

    public void UpdateSeed()
    {
        seed = ParseInt(Input_Seed.text, 0);
    }

    public void UpdateWidth()
    {
        Width = ParseInt(Input_Width.text, 0);
    }

    public void UpdateHeight()
    {
        Height = ParseInt(Input_Height.text, 0);
    }

    public void Download()
    {
        string MapName = string.Join("_", new string[]
        {
            "Map",
            currentType == NoiseType.Perlin ? "Perlin" :
                currentType == NoiseType.Billow ? "Billow" :
                currentType == NoiseType.RiggedMultifractal ? "RiggedMultiFractal" :
                "Voronoi",
            currentMapForm == MapType.Planar ? "Planar" :
                currentMapForm == MapType.Cylindrical ? "Cylindrical" :
                "Spherical",
            seed.ToString(),
            ("W" + Width),
            ("H" + Height)
        });

        print(MapName);

        #if UNITY_WEBGL
            ImageDownloader(System.Convert.ToBase64String(tex.EncodeToPNG()), MapName);
        #endif
    }

    public void Generate()
    {
        print("Generate");

        ModuleBase generator = GetModule(currentType);

        Noise2D map = new Noise2D(Width, Height, generator);

        switch (currentMapForm)
        {
            case MapType.Planar:
                map.GeneratePlanar(
                    south,
                    north,
                    west,
                    east);
                break;
            case MapType.Spherical:
                map.GenerateSpherical(
                    south,
                    north,
                    west,
                    east);
                break;
            case MapType.Cylindrical:
                map.GenerateCylindrical(
                    south,
                    north,
                    west,
                    east);
                break;
            default:
                break;
        }

        tex = map.GetTexture();
        tex.Apply();

        CommonMaterial.SetTexture("_BaseMap", tex);
    }

    #endregion

    #region Generator

    ModuleBase GetModule(NoiseType type)
    {
        ModuleBase Generator;

        switch (type)
        {

            case NoiseType.Perlin:
                Generator = new Perlin(
                    frequency,
                    lacunarity,
                    persistence,
                    octaves,
                    seed,
                    QualityMode.Low);

                break;
            case NoiseType.Billow:
                Generator = new Billow(
                    frequency,
                    lacunarity,
                    persistence,
                    octaves,
                    seed,
                    QualityMode.Low);

                break;
            case NoiseType.RiggedMultifractal:
                Generator = new RidgedMultifractal(
                    frequency,
                    lacunarity,
                    octaves,
                    seed,
                    QualityMode.Low);

                break;
            case NoiseType.Voronoi:
                Generator = new Voronoi(
                    frequency,
                    displacement,
                    seed,
                    Distance);

                break;
            default:
                Generator = new Perlin(
                    frequency,
                    lacunarity,
                    persistence,
                    octaves,
                    seed,
                    QualityMode.Low);

                break;
        }

        return Generator;
    }

    #endregion

    #region Parsers

    double ParseDouble(string val, double defaultValue)
    {
        System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Any;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

        double value;

        if (double.TryParse(val, styles, culture, out value))
        {
            return value;
        }

        return defaultValue;
    }

    int ParseInt(string val, int defaultValue)
    {
        int value;

        if (int.TryParse(val, out value))
        {
            return value;
        }

        return defaultValue;
    }

    #endregion
}
