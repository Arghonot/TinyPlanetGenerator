﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public PlanetProfile profile;
    public bool UseMaps;
    public int AmountOfVertices;
    public GameObject water;

    public CustomPerlinGenerator generator;
    MeshFilter filter;
    MeshRenderer render;

    public float length;
    Vector3[] __normals = null;
    Vector3[] __Vertices = null;

    #region UNITY

    private void Start()
    {
        if (!UseMaps)
        {
            Generate();
        }
    }

    #endregion

    #region REGENERATE

    public void Regenerate()
    {
        generator.mapSize = profile.TexturesSize;
        generator.Generate(profile);
        ReScale();

        if (UseMaps)
        {
            render.material = profile.material;

            // if lit
            if (render.material.shader.name == "Universal Render Pipeline/Lit")
            {
                //if (profile.name.Contains("Magma"))
                //{
                //    render.material.SetTexture("_EmissionMap", generator.InverseHeightMap);
                //}

                render.material.SetTexture("_BaseMap", generator.ColorMap);
            }
            else if (render.material.shader.name.Contains("Magma"))
            {
                render.material.SetTexture("Texture2D_D98FF2C8", generator.ColorMap);
            }
            else if (render.material.shader.name.Contains("PlanetGround"))
            {
                render.material.SetTexture("Texture2D_10E80854", generator.ColorMap);
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

        if (profile.useWater)
        {
            water.SetActive(true);
        }
        else
        {
            water.SetActive(false);
        }
    }

    #endregion

    #region INIT

    public void Generate()
    {
        generator = GetComponent<CustomPerlinGenerator>();

        Mesh mesh = new Mesh();

        SetupVerticesAndUV(mesh);

        mesh.triangles = ProcessTriangles();
        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        render = GetComponent<MeshRenderer>();

        //if (UseMaps)
        //{
        //    render.material.SetTexture("_BaseMap", generator.ColorMap);
        //}
    }

    #endregion

    #region Elevation

    void ReScale()
    {
        // We don't want to recalculate the elevation if there is none
        if (!UseMaps)
        {
            return;
        }

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

    //private void OnDrawGizmos()
    //{
    //    if (__normals != null)
    //    {
    //        for (int i = 0; i < __Vertices.Length; i++)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawLine(transform.position + __Vertices[i], transform.position + __Vertices[i] + (__normals[i] * length));
    //            Gizmos.color = Color.blue;
    //            Gizmos.DrawSphere(transform.position + __Vertices[i] + (__Vertices[i] * length), length / 20f);

    //            //Gizmos.DrawRay(new Ray(transform.position + __Vertices[i], transform.position + __Vertices[i] + (__normals[i] * length)));
    //        }
    //    }
    //}
}
