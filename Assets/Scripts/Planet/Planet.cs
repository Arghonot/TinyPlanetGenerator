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
    /// <summary>
    /// For debug purpose, change the ColorMap at runtime then ask a 
    /// "planet refresh" with this bool.
    /// </summary>
    public bool _customRegenerate;

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
    public Texture2D ColorMap;
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
        if (_customRegenerate)
        {
            Debug.Log("_customRegenerate");
            _customRegenerate = false;
            ColorMap = tex;

            ReScale();
            //SetGroundMaterialValues();

            //HandleWater();
            //HandleClouds();
            //HandleAura();
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
                ColorMap = new Texture2D(
                    profile.TexturesSize,
                    (int)(profile.TexturesSize / 2f));
                ColorMap.SetPixels(GeneratedTex.GetPixels());

                ReScale();
                SetGroundMaterialValues();

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
        Debug.Log("[INITIALIZE PLANET] " + gameObject.transform.parent.name);

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

        //mat = new Material(Shader.Find("Standard"));

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh " + i);
                meshObj.transform.parent = transform;

                meshRenderers[i] = meshObj.AddComponent<MeshRenderer>();
                //meshRenderers[i].sharedMaterial = mat;
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
    }

    #endregion

    #region Elevation

    void ReScale()
    {
        for (int i = 0; i < terrainFaces.Length; i++)
        {
            terrainFaces[i].ElevateMesh(
                ColorMap,
                .5f,
                profile.ElevationMultiplier,
                profile.ColorGradient);
        }
    }

    #endregion
}
