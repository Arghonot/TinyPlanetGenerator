using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NormalDebugger : MonoBehaviour
{
    public Mesh mesh;
    public float length;
    public Color color;

    public bool Run;

    void Update()
    {
        if (Run)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            Run = false;
        }
    }

    private void OnDrawGizmos()
    {
        //mesh = GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = color;

        if (mesh == null)
        {
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
