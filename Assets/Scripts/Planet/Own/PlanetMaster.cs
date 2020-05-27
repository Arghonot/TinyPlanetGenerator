using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkDatas
{
    // Width Length
    [SerializeField] public float Longitude;
    [SerializeField] public float Latitude;
    [SerializeField] public float Radius;
    // spherical rect begin
    [SerializeField] public float LongitudeBegin;
    [SerializeField] public float LatitudeBegin;
    // Mesh density
    [SerializeField] public int VerticesXAmount;
    [SerializeField] public int VerticesYAmount;

}

public class PlanetMaster : MonoBehaviour
{
    public int XAmount = 4;
    public int YAmount = 2;

    public Chunk Root = null;

    private void OnValidate()
    {
        if (Root == null)
        {
            CreateRoot();
        }
    }

    void CreateRoot()
    {
        GameObject root = new GameObject();

        root.name = "Root";
        root.transform.SetParent(transform);
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        Root = root.AddComponent<Chunk>();

        Root.Generate(new ChunkDatas()
        {
            Longitude = 360f,
            Latitude = 180f,
            LongitudeBegin = -180f,
            LatitudeBegin = -90f,
            VerticesXAmount = XAmount,
            VerticesYAmount = YAmount,
            Radius = 1f
        });
    }
}
