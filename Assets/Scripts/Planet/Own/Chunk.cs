using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    MeshRenderer rend;
    MeshFilter filter;

    Vector3[] vertices;
    Vector2[] uvs;

    public ChunkDatas datasSelf;

    public void Generate(ChunkDatas datas)
    {
        print(datas.VerticesXAmount + "     " + datas.VerticesYAmount);
        datasSelf = datas;

        rend = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = new Mesh();

        CreateMesh();
        print(datasSelf.VerticesXAmount + "     " + datasSelf.VerticesYAmount);
    }

    void CreateMesh()
    {
        float lnStep = datasSelf.Longitude / datasSelf.VerticesXAmount;
        float latStep = datasSelf.Latitude / datasSelf.VerticesYAmount;

        vertices = new Vector3[(datasSelf.VerticesXAmount + 1) * (datasSelf.VerticesYAmount + 1)];
        uvs = new Vector2[vertices.Length];

        for (int x = 0, i = 0; x <= datasSelf.VerticesXAmount; x++)
        {
            for (int y = 0; y <= datasSelf.VerticesYAmount; y++, i++)
            {
                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    (datasSelf.LongitudeBegin + (x * lnStep)) * Mathf.Deg2Rad,
                    (datasSelf.LatitudeBegin + (y * latStep)) * Mathf.Deg2Rad,
                    datasSelf.Radius);
                uvs[i] = new Vector2(
                    (float)x / (float)datasSelf.VerticesXAmount,
                    (float)y / (float)datasSelf.VerticesYAmount);
            }
        }

        filter.sharedMesh.Clear();
        filter.sharedMesh.vertices = vertices;
        filter.sharedMesh.uv = uvs;
        filter.sharedMesh.triangles = CalculateTriangles();
    }

    int[] CalculateTriangles() 
    {
        int[] triangles = new int[(datasSelf.VerticesXAmount) * (datasSelf.VerticesYAmount) * 6];

        for (int ti = 0, vi = 0, y = 0; y < datasSelf.VerticesYAmount; y++, vi++)
        {
            for (int x = 0; x < datasSelf.VerticesXAmount; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + datasSelf.VerticesXAmount + 1;
                triangles[ti + 5] = vi + datasSelf.VerticesXAmount + 2;

                print(
                    (ti + 0) + "    [1]" +
                    (ti + 1) + "    [2]" +
                    (ti + 2) + "    [3]" +
                    (ti + 3) + "    [4]" +
                    (ti + 4) + "    [5]" +
                    (ti + 5));
            }
        }

        return triangles;
    }
}
