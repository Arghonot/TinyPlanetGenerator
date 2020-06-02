using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTest : MonoBehaviour
{
    public Texture2D tex;
    [Range(2, 256)]
    public int resolution = 10;

    public float meanElevation;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;


    float south = 90.0f;
    float north = -90.0f;

    float west = -180.0f;
    float east = 180.0f;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
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

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].sharedMesh.name = i.ToString();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        Perlin perlin = new Perlin(1d, 2d, .1d, 6, 7, QualityMode.High);
        //Voronoi voro = new Voronoi(1, 2, 4, false);
        //Billow billow = new Billow(0.9d, 1000, 0.1, 1, 42, QualityMode.Low);
        //Turbulence turb = new Turbulence(.3d, perlin);
        //Noise2D noise = new Noise2D(512, 256, turb);
        Noise2D noise = new Noise2D(512, 256, perlin);
        noise.GenerateSpherical(
            south,
            north,
            west,
            east);

        tex = noise.GetTexture();
        tex.Apply();

        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }

        for (int i = 0; i < terrainFaces.Length; i++)
        {
            //terrainFaces[i].ElevateMesh(tex, meanElevation);
        }
    }
}
