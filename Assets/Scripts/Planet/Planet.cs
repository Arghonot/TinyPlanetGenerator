using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public PlanetProfile profile;

    public Rotator Anchor;

    public GameObject water;
    public GameObject Aura;
    public Cloud Clouds;

    public CustomPerlinGenerator generator;

    public int AmountOfVertices;
    MeshFilter filter;
    MeshRenderer render;

    #region UNITY

    #endregion

    #region REGENERATE

    public void Regenerate()
    {
        generator.mapSize = profile.TexturesSize;
        generator.Generate(profile);
        ReScale();

        SetGroundMaterialValues();

        HandleWater();
        HandleClouds();
        HandleAura();
    }

    /// <summary>
    /// Setup everything related to the planet's water (should it be activated, colors, ...).
    /// </summary>
    void HandleWater()
    {
        if (profile.useWater)
        {
            water.SetActive(true);

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
            auramat.SetFloat("Vector1_F80876CD", profile.AuraIntensity);
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
        render.material = profile.material;

        // if lit
        if (render.material.shader.name == "Universal Render Pipeline/Lit")
        {
            render.material.SetTexture("_BaseMap", generator.ColorMap);
        }
        else if (render.material.shader.name.Contains("Magma"))
        {
            render.material.SetTexture("Texture2D_D98FF2C8", generator.ColorMap);
        }
        else if (render.material.shader.name.Contains("PlanetGround"))
        {
            render.material.SetTexture("Texture2D_10E80854", generator.ColorMap);

            render.material.SetFloat("Vector1_AC9353BB", profile.CliffIntensity);

            render.material.SetColor("Color_5EBF6256", profile.CliffLightColor);
            render.material.SetColor("Color_863EF5B8", profile.CliffDarkColor);
        }
        else if (render.material.shader.name.Contains("WeirdFresnel"))
        {
            render.material.SetTexture("Texture2D_C9B692E6", generator.ColorMap);
        }
        // if unlit
        else
        {
            render.material.SetTexture("_MainTex", generator.ColorMap);
        }
    }

    #endregion

    #region INIT

    /// <summary>
    /// Initialize the planet, create it's mesh and base uv + required components.
    /// </summary>
    public void Initialize()
    {
        generator = GetComponent<CustomPerlinGenerator>();

        Mesh mesh = new Mesh();

        SetupVerticesAndUV(mesh);

        mesh.triangles = ProcessTriangles();
        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        render = GetComponent<MeshRenderer>();
    }

    #endregion

    #region Elevation

    void ReScale()
    {
        Vector3[] vertices = new Vector3[
            ((AmountOfVertices + 1) * (AmountOfVertices + 1))];
        Vector3[] normals = new Vector3[vertices.Length];
        float latPadding = 180f / (float)AmountOfVertices;
        float lonPadding = 360f / (float)AmountOfVertices;

        for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        {
            for (int y = 0; y <= AmountOfVertices; y++, i++)
            {
                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    (((x * lonPadding) - 180f) * Mathf.Deg2Rad),
                    (((y * latPadding) - 90f) * Mathf.Deg2Rad),
                    profile.BaseElevation + (GetGrayScale(
                            generator.HeightMap,
                            x,
                            y) * profile.ElevationMultiplier));
            }
        }

        //for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        //{
        //    for (int y = 0; y <= AmountOfVertices; y++, i++)
        //    {
        //        normals[i] = getNormal(i, vertices);
        //    }
        //}

        filter.mesh.vertices = vertices;
        filter.mesh.RecalculateNormals();
        Vector3[] copied_normals = filter.mesh.normals;
        copied_normals = CalCulateNormals(copied_normals, vertices);
        filter.mesh.RecalculateTangents();
        filter.mesh.normals = copied_normals;
    }

    Vector3[] CalCulateNormals(Vector3[] normals, Vector3[] vertices)
    {
        for (int i = 0; i <= AmountOfVertices; i++)
        {
            int endindex = ((1 + AmountOfVertices) * (1 + AmountOfVertices)) - (AmountOfVertices) + i - 1;

            Vector3 leftvector = vertices[endindex] + vertices[endindex - 1];
            Vector3 rightvector = vertices[i] + vertices[i + 1];
            Vector3 newnormal = (leftvector + rightvector).normalized;

            if (newnormal.magnitude < 0)
                newnormal *= -1f;

            normals[i] = normals[endindex] = newnormal;
        }

        return normals;
    }

    float GetGrayScale(Texture2D tex, int x, int y)
    {
        if (x == AmountOfVertices && y == AmountOfVertices)
        {
            return tex.GetPixel(tex.width - 1, tex.height - 1).grayscale;
        }

        return tex.GetPixel(
            (int)((float)tex.width * ((float)x / (float)AmountOfVertices)),
            (int)((float)tex.height * ((float)y / (float)AmountOfVertices))).grayscale;
    }

    #endregion

    #region MeshGeneration

    void SetupVerticesAndUV(Mesh mesh)
    {
        Vector3[] vertices = new Vector3[
            ((AmountOfVertices + 1) * (AmountOfVertices + 1))];
        Vector2[] uv = new Vector2[vertices.Length];
        float latPadding = 180f / (float)AmountOfVertices;
        float lonPadding = 360f / (float)AmountOfVertices;

        for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        {
            for (int y = 0; y <= AmountOfVertices; y++, i++)
            {
                    vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                        (((x * lonPadding) - 180f) * Mathf.Deg2Rad),
                        (((y * latPadding) - 90f) * Mathf.Deg2Rad),
                        .96f);

                //print("[X][DEG]" + x + " " + (x * lonPadding) + "    [Y][DEG]" + y + " " + (y * latPadding));

                uv[i] = new Vector2(
                    (float)x / (float)AmountOfVertices,
                    (float)y / (float)AmountOfVertices);
            }
        }

        mesh.RecalculateNormals();
        mesh.vertices = vertices;
        mesh.uv = uv;
    }

    int[] ProcessTriangles()
    {
        int[] triangles = new int[((AmountOfVertices * AmountOfVertices) + AmountOfVertices) * 6];

        for (int ti = 0, vi = 0, y = 0; y < AmountOfVertices; y++, vi++)
        {
            for (int x = 0; x < AmountOfVertices; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + AmountOfVertices + 1;
                triangles[ti + 5] = vi + AmountOfVertices + 2;
            }
        }

        return triangles;
    }

    #endregion
}
