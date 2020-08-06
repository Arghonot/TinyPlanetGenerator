using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDebuger : MonoBehaviour
{
    public MeshFilter filter;
    public float size;
    public int Res;

    public bool debugVertices;
    public bool debugNormals;
    public bool bottom;
    public bool left;
    public bool right;
    public bool top;

    public Color Color = Color.red;

    private void OnDrawGizmos()
    {
        if (filter == null) return;

        Vector3[] normals = filter.sharedMesh.normals;
        Vector3[] vertices = filter.sharedMesh.vertices;

        for (int i = 0; i < normals.Length; i++)
        {
            if (debugVertices)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(vertices[i], vertices[i] + (vertices[i] * size));
            }
        }
        if (debugNormals)
        {
            Gizmos.color = Color;

            int index = 0;
            for (int i = 0; i < Res; i++)
            {
                if (bottom)
                {
                    index = i;
                    Gizmos.DrawLine(vertices[index], vertices[index] + (normals[index] * size));
                }
                if (left)
                {
                    index = i * Res;
                    Gizmos.DrawLine(vertices[index], vertices[index] + (normals[index] * size));
                }
                if (right)
                {
                    index = (i * Res) + Res - 1;
                    Gizmos.DrawLine(vertices[index], vertices[index] + (normals[index] * size));
                }
                if (top)
                {
                    index = (Res * Res) - Res + i;
                    Gizmos.DrawLine(vertices[index], vertices[index] + (normals[index] * size));
                }
            }
        }
    }
}
