using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public struct NodeMeshData
{
    public int level;
    public Vector2 MeshSize;
    public Vector2 BeginCoordinates;
    public Vector2 LengthCoordinates;
}

public class MeshNode : MonoBehaviour
{
    public NodeMeshData datas;

    public MeshNode Father;
    public MeshNode[] Sons = null;

    public Transform trans;
    public MeshRenderer rend;
    public Vector3 MeshCenter;

    private void OnValidate()
    {
        Generate(new NodeMeshData()
        {
            level = 1,
            MeshSize = new Vector2(80, 30),
            BeginCoordinates = new Vector2(180f, -90f),
            LengthCoordinates = new Vector2(360f, 180f)
        });
    }

    public void Generate(NodeMeshData data)
    {
        datas = data;
        trans = transform;

        var Filter = gameObject.AddComponent<MeshFilter>();
        Filter.mesh = new Mesh();
        rend = gameObject.AddComponent<MeshRenderer>();

        var vertices = new Vector3[((int)datas.MeshSize.x + 1) * ((int)datas.MeshSize.y + 1)];
        var triangles = new int[(int)datas.MeshSize.x * (int)datas.MeshSize.y * 6];
        var uv = new Vector2[vertices.Length];
        var normal = new Vector3[vertices.Length];
        float longitude;
        float latitude;

        for (int i = 0, y = 0; y <= (int)datas.MeshSize.y; y++)
        {
            for (int x = 0; x <= (int)datas.MeshSize.x; x++, i++)
            {
                if (datas.level == 0)
                {
                    longitude = CoordinatesProjector.GetLongitude(x, (int)data.MeshSize.x) * (Mathf.PI / 180.0f);
                    latitude = CoordinatesProjector.GetLatitude(y, (int)data.MeshSize.y) * (Mathf.PI / 180.0f);
                }
                longitude = CoordinatesProjector.GetLongitudeFromPositions(x, (int)datas.MeshSize.x, datas.BeginCoordinates.x, datas.LengthCoordinates.x) * (Mathf.PI / 180.0f);
                latitude = CoordinatesProjector.GetLatitudeFromPositions(y, (int)datas.MeshSize.y, datas.BeginCoordinates.y, datas.LengthCoordinates.y) * (Mathf.PI / 180.0f);

                vertices[i] = CoordinatesProjector.InverseMercatorProjector(longitude, latitude, 1f);//new Vector3(x,0, y);

                uv[i] = new Vector2((float)x / (int)datas.MeshSize.x, (float)y / (int)datas.MeshSize.y);
                normal[i] = vertices[i] - transform.position;
            }
        }

        Filter.mesh.vertices = vertices;
        Filter.mesh.uv = uv;
        Filter.mesh.normals = normal;

        for (int ti = 0, vi = 0, y = 0; y < (int)datas.MeshSize.y; y++, vi++)
        {
            for (int x = 0; x < (int)datas.MeshSize.x; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + (int)datas.MeshSize.x + 1;
                triangles[ti + 5] = vi + (int)datas.MeshSize.x + 2;
            }
        }
        Filter.mesh.triangles = triangles;
        MeshCenter = vertices[vertices.Length / 2];
    }
}
