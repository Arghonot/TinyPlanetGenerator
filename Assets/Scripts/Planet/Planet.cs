using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// V(x) = ((x - 2) * x ) + 2
// T(x) = (x * 2) + ((x - 3) * (x * 2))

public class PlanetGenerationData
{
    public int PGDMapSize;
    public PlanetProfile PGDProfile;
    public Action<Texture2D> PGDCallback;
}

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    #region Fields

    #region Mesh management

    [Range(2, 256)]
    public int resolution = 10;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshRenderer[] meshRenderers;
    TerrainFace[] terrainFaces;

    #endregion

    #region Base arguments

    public float meanElevation = .15f;
    public float baseElevation = .5f;

    #endregion

    #region GameObject management

    public Rotator Anchor;

    public GameObject water;
    public GameObject Aura;
    public Cloud Clouds;

    #endregion

    #region Planet rendering

    public PlanetProfile profile;
    Material mat;
    public Texture2D ColorMap
    {
        get
        {
            return _colorMap;
        }
        set
        {
            _colorMap = new Texture2D(value.width, value.height);
            _colorMap.SetPixels(value.GetPixels());
            _colorMap.Apply();
        }
    }
    Texture2D _colorMap;

    #endregion

    #region DEBUG

    public Texture2D DebugMap;
    public Material DebugMaterial;
    /// <summary>
    /// For debug purpose, change the ColorMap at runtime then ask a 
    /// "planet refresh" with this bool.
    /// </summary>
    public bool _customRegenerate;
    public bool save;

    int _oldResolution;

    #endregion

    #endregion

    #region Functions

    #region UNITY

    private void Update()
    {
        if (save)
        {
            SavePlanet();
        }
        if (_customRegenerate)
        {
            CustomRegenerate();
        }
    }

    #endregion

    #region REGENERATE

    public void Regenerate()
    {
        Debug.Log("[REGENERATE] " + gameObject.transform.parent.name);

        PlanetGenerationData PGDDatas = new PlanetGenerationData()
        {
            PGDMapSize = profile.TexturesSize,
            PGDProfile = this.profile,
            PGDCallback = (Texture2D GeneratedTex) =>
            {
                _colorMap = new Texture2D(
                    profile.TexturesSize,
                    (int)(profile.TexturesSize / 2f));
                _colorMap.SetPixels(GeneratedTex.GetPixels());
                _colorMap.Apply();

                SetGroundActive(profile.UseGround);

                if (profile.UseGround)
                {
                    Elevate();
                    SetGroundMaterialValues();
                }

                HandleWater();
                HandleClouds();
                HandleAura();
            }
        };
        
        CustomPerlinGenerator.Instance.Generate(PGDDatas);
    }

    /// <summary>
    /// Setup everything related to the planet's water (should it be activated, colors, ...).
    /// </summary>
    void HandleWater()
    {
        if (profile == null) return;

        water.SetActive(profile.UseWater);

        if (profile.UseWater)
        {
            water.transform.localScale = Vector3.one * profile.SeaLevel;
            Material WaterMaterial = 
                (water.GetComponent<MeshRenderer>().material = profile.WaterMaterial);

            if (profile.WaterMaterial.shader.name.Contains("Water"))
            { 
                WaterMaterial.SetColor("Color_109BE5B1", profile.WaterColor);
                WaterMaterial.SetColor("Color_B902B901", profile.RippleColor);

            }
            else if (profile.WaterMaterial.shader.name.Contains("Magma"))
            {
                WaterMaterial.SetTexture(
                    "Texture2D_D98FF2C8",
                    _colorMap);
                WaterMaterial.SetColor("Color_F4940654", profile.SunFresnelColor);
            }
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
                _colorMap);
        }
        else if (mat.shader.name.Contains("Magma"))
        {
            mat.SetTexture(
                "Texture2D_D98FF2C8",
                _colorMap);
            mat.SetColor("Color_F4940654", profile.SunFresnelColor);
        }
        else if (mat.shader.name.Contains("PlanetGround"))
        {
            mat.SetTexture(
                "Texture2D_10E80854",
                _colorMap);

            mat.SetFloat("Vector1_AC9353BB", profile.CliffIntensity);

            mat.SetColor("Color_5EBF6256", profile.CliffLightColor);
            mat.SetColor("Color_863EF5B8", profile.CliffDarkColor);
        }
        else if (mat.shader.name.Contains("EnhancedGroundV2"))
        {
            mat.SetTexture(
                "Texture2D_4FCE4029",
                _colorMap);
        }
        else if (mat.shader.name.Contains("WeirdFresnel"))
        {
            mat.SetTexture(
                "Texture2D_C9B692E6",
                _colorMap);
        }
        // if unlit
        else
        {
            mat.SetTexture(
                "_MainTex",
                _colorMap);
        }
    }

    void SetGroundActive(bool isActive)
    {
        for (int i = 0; i < terrainFaces.Length; i++)
        {
            terrainFaces[i].gameObject.SetActive(isActive);
        }
    }

    #endregion
    
    #region INIT

    /// <summary>
    /// Initialize the planet, create it's mesh and base uv + required components.
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[INITIALIZE PLANET] ");

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

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh " + i);
                meshObj.transform.parent = transform;

                meshRenderers[i] = meshObj.AddComponent<MeshRenderer>();
                meshRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].sharedMesh.name = i.ToString();
                meshObj.transform.localPosition = Vector3.zero;
            }

            terrainFaces[i] = meshFilters[i].gameObject.AddComponent<TerrainFace>();
                
            terrainFaces[i].InitTerrainFace(
                meshFilters[i].sharedMesh,
                resolution,
                directions[i]);
        }

        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }

        _oldResolution = resolution;
    }

    #endregion

    #region Elevation

    public void Elevate()
    {
        if (profile == null)
        {
            for (int i = 0; i < terrainFaces.Length; i++)
            {
                terrainFaces[i].ElevateMesh(
                    ColorMap,
                    baseElevation,
                    meanElevation);
            }

            return;
        }

        for (int i = 0; i < terrainFaces.Length; i++)
        {
            terrainFaces[i].ElevateMesh(
                _colorMap,
                profile.BaseElevation,
                profile.ElevationMultiplier);
        }
    }

    #endregion

    #region DEBUG

    void SavePlanet()
    {
        save = false;

        var bytes = _colorMap.EncodeToPNG();
        //System.IO.File.WriteAllBytes(
        //    "C:/Users/loriv/OneDrive/Pictures/Tiny_planet_generator/GeneratedPlanet/planet.png", bytes);
        File.WriteAllBytes(Application.dataPath + "/Textures/TestMaps/PlanetRendered.png", bytes);
    }

    void CustomRegenerate()
    {
        Debug.Log("_customRegenerate");
        _customRegenerate = false;

        if (terrainFaces == null ||
            terrainFaces.Length == 0 ||
            terrainFaces[0] == null)
        {
            Initialize();

            if (meshRenderers[0].sharedMaterial == null)
            {
                if (DebugMaterial == null)
                {
                    DebugMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                }

                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].sharedMaterial = DebugMaterial;
                }
            }
        }
        else if (_oldResolution != resolution)
        {
            for (int i = 0; i < terrainFaces.Length; i++)
            {
                terrainFaces[i].resolution = resolution;
                terrainFaces[i].ConstructMesh();
                meshFilters[i].mesh = terrainFaces[i].mesh;
            }

            _oldResolution = resolution;
        }

        ColorMap = DebugMap;

        Elevate();
    }

    #endregion

    #endregion
}
