using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetTester : MonoBehaviour
{
    public Gradient grad;
    public bool Regenerate = false;
    public Texture2D tex;
    [Range(2, 256)]
    public int resolution = 10;

    public float meanElevation;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    private void Update()
    {
        if (Regenerate)
        {
            for (int i = 0; i < terrainFaces.Length; i++)
            {
                terrainFaces[i].ElevateMesh(tex, .5f, meanElevation);
            }

            Regenerate = false;
        }
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

            var face = meshFilters[i].GetComponent<TerrainFace>();

            if (face == null)
            {
                terrainFaces[i] = meshFilters[i].gameObject.AddComponent<TerrainFace>();
            }
            else
            {
                terrainFaces[i] = face;
            }
            
            terrainFaces[i].InitTerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }

        for (int i = 0; i < terrainFaces.Length; i++)
        {
            terrainFaces[i].ElevateMesh(tex, .5f, meanElevation, grad);
        }
    }
}
