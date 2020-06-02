using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDebugger : MonoBehaviour
{
    public Mesh mesh;
    public float length;

    private void OnDrawGizmos()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        if (mesh == null)
        {
            return;
        }

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Gizmos.DrawLine(
                mesh.vertices[i],
                mesh.vertices[i] + (mesh.normals[i] * length));
        }
    }
}
