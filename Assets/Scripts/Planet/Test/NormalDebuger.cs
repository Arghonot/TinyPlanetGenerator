using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDebuger : MonoBehaviour
{
    public MeshFilter filter;
    public float size;

    public bool debugVertices;
    public bool debugNormals;

    private void OnDrawGizmosSelected()
    {
        if (filter == null) return;

        Vector3[] normals = filter.sharedMesh.normals;
        Vector3[] vertices = filter.sharedMesh.vertices;

        for (int i = 0; i < normals.Length; i++)
        {
            if (debugNormals)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(vertices[i], vertices[i] + (normals[i] * size));
            }
            if (debugVertices)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(vertices[i], vertices[i] + (vertices[i] * size));
            }
        }
    }
}
