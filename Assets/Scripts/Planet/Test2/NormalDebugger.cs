using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDebugger : MonoBehaviour
{
    public Mesh mesh;
    public float length;
    public Color color;

    private void OnDrawGizmos()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = color;

        if (mesh == null)
        {
            print("yeyeyey");
            return;
        }

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Gizmos.DrawLine(
                transform.position + mesh.vertices[i],
                transform.position + mesh.vertices[i] + (mesh.normals[i] * length));
        }
    }
}
