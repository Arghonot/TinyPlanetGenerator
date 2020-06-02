using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// V(x) = ((x - 2) * x ) + 2
// T(x) = (x * 2) + ((x - 3) * (x * 2))

public class PlanetGenerationData
{
    public int mapSize;
    public PlanetProfile profile;
    public Action Callback;
}

public class Planet : MonoBehaviour
{
    public Texture2D tex;
    [Range(2, 256)]
    public int resolution = 10;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshRenderer[] meshRenderers;
    TerrainFace[] terrainFaces;

    public PlanetProfile profile;

    public Rotator Anchor;

    public GameObject water;
    public GameObject Aura;
    public Cloud Clouds;

    Material mat;
    Texture2D ColorMap;
    Texture2D HeightMap;
    public bool save;

    #region UNITY

    private void Update()
    {
        if (save)
        {
            save = false;

            var bytes = ColorMap.EncodeToPNG();
            System.IO.File.WriteAllBytes(
                "C:/Users/loriv/OneDrive/Pictures/Tiny_planet_generator/GeneratedPlanet/planet.png", bytes);
        }
    }

    #endregion

    #region REGENERATE

    public void Regenerate()
    {
        //CustomPerlinGenerator.Instance.mapSize = profile.TexturesSize;
        //CustomPerlinGenerator.Instance.Generate(profile);

        CustomPerlinGenerator.Instance.Generate(new PlanetGenerationData()
        {
            mapSize = profile.TexturesSize,
            profile = this.profile,
            Callback = () =>
            {
                ColorMap = CustomPerlinGenerator.Instance.ColorMap;
                HeightMap = CustomPerlinGenerator.Instance.HeightMap;

                ReScale();
                SetGroundMaterialValues();

                HandleWater();
                HandleClouds();
                HandleAura();
            }    
        });
    }

    /// <summary>
    /// Setup everything related to the planet's water (should it be activated, colors, ...).
    /// </summary>
    void HandleWater()
    {
        if (profile.useWater)
        {
            water.SetActive(true);

            water.transform.localScale = Vector3.one * profile.SeaLevel;

            Material Watermaterial = water.GetComponent<MeshRenderer>().material;

            Watermaterial.SetColor("Color_109BE5B1", profile.WaterColor);
            Watermaterial.SetColor("Color_B902B901", profile.RippleColor);
        }
        else
        {
            water.SetActive(false);
        }
    }

    /// <summary>
    /// Setup everything related to the planet's clouds (should it be activated, colors, ...).
    /// </summary>
    void HandleClouds()
    {
        if (profile.UseClouds)
        {
            Clouds.gameObject.SetActive(true);
            Clouds.Generate();
        }
        else
        {
            Clouds.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Setup everything related to the planet's atmosphere (should it be activated, colors, ...)..
    /// </summary>
    void HandleAura()
    {
        if (profile.UseAura)
        {
            Aura.SetActive(true);
            Material auramat = Aura.GetComponent<MeshRenderer>().material;

            auramat.SetColor("Color_C8024A4F", profile.Aura);
            auramat.SetFloat("Vector1_733A9945", profile.AuraIntensity);
        }
        else
        {
            Aura.SetActive(false);
        }
    }

    /// <summary>
    /// Setup the correct material for the profile and it's own variables.
    /// </summary>
    void SetGroundMaterialValues()
    {
        mat = profile.material;

        foreach (var face in meshRenderers)
        {
            face.sharedMaterial = mat;
        }

        // if lit
        if (mat.shader.name == "Universal Render Pipeline/Lit")
        {
            mat.SetTexture(
                "_BaseMap",
                ColorMap);
        }
        else if (mat.shader.name.Contains("Magma"))
        {
            mat.SetTexture(
                "Texture2D_D98FF2C8",
                ColorMap);
            mat.SetColor("Color_F4940654", profile.SunFresnelColor);
        }
        else if (mat.shader.name.Contains("PlanetGround"))
        {
            mat.SetTexture(
                "Texture2D_10E80854",
                ColorMap);

            mat.SetFloat("Vector1_AC9353BB", profile.CliffIntensity);

            mat.SetColor("Color_5EBF6256", profile.CliffLightColor);
            mat.SetColor("Color_863EF5B8", profile.CliffDarkColor);
        }
        else if (mat.shader.name.Contains("EnhancedGroundV2"))
        {
            mat.SetTexture(
                "Texture2D_4FCE4029",
                ColorMap);
        }
        else if (mat.shader.name.Contains("WeirdFresnel"))
        {
            mat.SetTexture(
                "Texture2D_C9B692E6",
                ColorMap);
        }
        // if unlit
        else
        {
            mat.SetTexture(
                "_MainTex",
                ColorMap);
        }
    }

    #endregion
    
    #region INIT

    /// <summary>
    /// Initialize the planet, create it's mesh and base uv + required components.
    /// </summary>
    public void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            meshRenderers = new MeshRenderer[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        mat = new Material(Shader.Find("Standard"));

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh " + i);
                meshObj.transform.parent = transform;

                meshRenderers[i] = meshObj.AddComponent<MeshRenderer>();
                meshRenderers[i].sharedMaterial = mat;
                meshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].sharedMesh.name = i.ToString();
                meshObj.transform.localPosition = Vector3.zero;
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }

        //for (int i = 0; i < terrainFaces.Length; i++)
        //{
        //    terrainFaces[i].ConstructMesh();
        //    meshFilters[i].gameObject.transform.position = Vector3.zero;
        //}

        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    #endregion

    #region Elevation

    void ReScale()
    {
        for (int i = 0; i < terrainFaces.Length; i++)
        {
            terrainFaces[i].ElevateMesh(HeightMap,profile.BaseElevation, profile.ElevationMultiplier);
        }
    }

    #endregion
}
