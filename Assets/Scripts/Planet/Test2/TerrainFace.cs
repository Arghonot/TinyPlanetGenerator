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
    Vector3[] OutOfBoundVertices;
    int borderTriangleIndex;
    int[] borderTriangles;

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
        Vector3[] vertices = new Vector3[resolution * resolution];
        OutOfBoundVertices = new Vector3[resolution * 4 + 4];
        borderTriangles = new int[24 * resolution];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        Vector3 Min = localUp - 0.5f * 2 * axisA - .5f * 2 * axisB;
        Vector3 Max = localUp + 0.5f * 2 * axisA + .5f * 2 * axisB;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                uvs[i] = percent;
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = pointOnUnitSphere;

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

        //for (int y = 0; y < resolution + 2; y++)
        //{
        //    for (int x = 0; x < resolution + 2; x++)
        //    {
        //        int i = x + y * resolution;
        //        Vector2 percent = new Vector2(x, y) / (resolution - 1);
        //        Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
        //        Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

        //        OutOfBoundVertices[i] = pointOnUnitSphere;
        //    }
        //}

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            OutOfBoundVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
        }
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

        //for (int x = 0, i = 0; x < resolution + 2; x++)
        //{
        //    for (int y = 0; y < resolution + 2; y++, i++)
        //    {
        //        ln = CoordinatesProjector.CartesianToLon(OutOfBoundVertices[i].normalized);
        //        lat = CoordinatesProjector.CartesianToLat(OutOfBoundVertices[i].normalized);

        //        intensity = 1 + (GetGrayScale(noise, ln + 180f, lat + 90f) * meanElevation);

        //        OutOfBoundVertices[i] = CoordinatesProjector.InverseMercatorProjector(
        //            ln * Mathf.Deg2Rad,
        //            lat * Mathf.Deg2Rad,
        //            intensity);
        //    }
        //}

        mesh.vertices = vertices;
        mesh.normals = CalculateNormals();
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

        //for (int i = 0; i < resolution; i++)
        //{
        //    vertexNormals[i] = SurfaceNormalFromIndices(
        //        vertices[i],
        //        OutOfBoundVertices[i + 2],
        //        vertices[i + 1]);
        //}

        //for (int i = vertices.Length; i > 0; i--)
        //{
        //    if (mesh.name.Contains("0"))
        //    {
        //        Debug.Log("[i][vertexNormals index][OutOfBoundVertices length][vertices length][" + 
        //            i + "][" +
        //            (vertices.Length - (resolution + 1) + i) + "][" +
        //            OutOfBoundVertices.Length + "]" +
        //            vertices.Length + "]");
        //    }

        //    vertexNormals[i] = SurfaceNormalFromIndices(
        //        vertices[i],
        //        OutOfBoundVertices[i + 2],
        //        vertices[vertices.Length + (resolution + 1) + i + 1]);
        //}

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