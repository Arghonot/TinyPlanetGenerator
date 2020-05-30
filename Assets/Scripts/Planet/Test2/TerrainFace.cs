using LibNoise;
using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    public Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    Vector3[] vertices;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        // we make sure resolution becomes pair
        // so a 3x3 stays a 3x3 visually but become a 4x4
        if (resolution % 2 != 0)
        {
            resolution += 1;
        }

        //Debug.Log(resolution);

        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(resolution - 1) * (resolution) * 6];
        int triIndex = 0;
        float ln = 0f;
        float lat = 0f;
        float lnOffset = 360f / (float)((resolution - 1) * 4);
        float latOffset = 180f / (float)((resolution - 1) * 4);
        //int i = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;

                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                if (mesh.name.Contains("2"))
                {
                    if (y == (resolution / 2) - 1 || y == resolution / 2)
                    {
                        percent = new Vector2(percent.x, .5f);
                    }
                }
                else if (mesh.name.Contains("4") || mesh.name.Contains("5"))
                {
                    if (x == (resolution / 2) - 1 || x == resolution / 2)
                    {
                        percent = new Vector2(.5f, percent.y);
                    }
                }

                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                ln = CoordinatesProjector.CartesianToLon(pointOnUnitSphere);
                lat = CoordinatesProjector.CartesianToLat(pointOnUnitSphere);

                vertices[i] = pointOnUnitSphere;
                uvs[i] = new Vector2(
                    ((ln + 180f) / (360f)),
                    ((lat + 90f) / (180f)));

                if ((ln <= -180f || ln >= 180f) ||
                    (lat <= -180f || lat >= 180f))
                {
                    // y pole face
                    if (mesh.name.Contains("5"))
                    {
                        if (x == resolution / 2)
                        {
                            uvs[i] = new Vector2(0f, uvs[i].y);
                        }
                    }
                    // middle face where there is a separation
                    if (mesh.name.Contains("2"))
                    {
                        if (y == resolution / 2)
                        {
                            uvs[i] = new Vector2(0f, uvs[i].y);
                        }
                    }
                    // -y pole face
                    if (mesh.name.Contains("4"))
                    {
                        if (x == resolution / 2)
                        {
                            uvs[i] = new Vector2(1f, uvs[i].y);
                        }
                        if (x == (resolution / 2) - 1)
                        {
                            uvs[i] = new Vector2(0f, uvs[i].y);
                        }
                    }
                }

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void ElevateMesh(Texture2D noise, float meanElevation)
    {
        vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];

        float ln = 0f;
        float lat = 0f;
        float intensity = 0f;

        for (int x = 0, i = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++, i++)
            {
                ln = CoordinatesProjector.CartesianToLon(vertices[i].normalized);
                lat = CoordinatesProjector.CartesianToLat(vertices[i].normalized);

                intensity = 1 + (GetGrayScale(noise, ln + 180f, lat + 90f) * meanElevation);

                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    ln * Mathf.Deg2Rad,
                    lat * Mathf.Deg2Rad,
                    intensity);
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
//        mesh.normals = CalculateNormals();
    }

    Vector3 SurfaceNormalFromIndices(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        
        int triangleCount = mesh.triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = mesh.triangles[normalTriangleIndex];
            int vertexIndexB = mesh.triangles[normalTriangleIndex + 1];
            int vertexIndexC = mesh.triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(
                vertices[vertexIndexA],
                vertices[vertexIndexB],
                vertices[vertexIndexC]);

            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    float GetGrayScale(Texture2D tex, float ln, float la)
    {
        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f))).grayscale;
    }
}