using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HeightSampler : MonoBehaviour
{
    public bool Run;
    public int X;
    public int Y;
    public int  Resolution;
    public Vector3 localUp;
    public Vector3 axisA;
    public Vector3 axisB;
    public float MeanElevation;
    public float BaseElevation;
    public Texture2D tex;

    public float size;

    MeshFilter filter;

    List<Transform> Points;

    void Start()
    {
        filter = GetComponent<MeshFilter>();

        if (Points == null || Points.Count == 0)
        {
            Init();
        }
    }

    void Update()
    {        
        if (Run)
        {
            if (filter != null)
            {
                SampleSurroundingPositions();
            }

            Run = false;
        }
    }

    void SampleSurroundingPositions()
    {
        ResizePoints();

        Points.ForEach(x => x.transform.position = 
            filter.mesh.vertices[(X * Resolution) + Y]);
    }

    Vector3 GetSpherifiedPositionFromXY(int x, int y)
    {
        Vector3 pos = GetVerticePosition(GetPercentage(x, y));
        float ln = CoordinatesProjector.CartesianToLon(pos);
        float lat = CoordinatesProjector.CartesianToLat(pos);
        float intensity = BaseElevation + (GetGrayScale(ln + 180f, lat + 90f) * MeanElevation);

        Vector3 newpos = CoordinatesProjector.InverseMercatorProjector(
            ln * Mathf.Deg2Rad,
            lat * Mathf.Deg2Rad,
            intensity);

        return newpos;
    }

    public Vector3 GetVerticePosition(Vector2 percent)
    {
        return (localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB).normalized;
    }

    public Vector2 GetPercentage(int x, int y)
    {
        return new Vector2(x, y) / (Resolution - 1);
    }

    float GetGrayScale(float ln, float la)
    {
        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f))).grayscale;
    }



    void ResizePoints()
    {
        Points.ForEach(x => x.transform.localScale = Vector3.one * size);
    }

    void Init()
    {
        Points = new List<Transform>();

        for (int i = 0; i < 8; i++)
        {
            var pt = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pt.name = i.ToString();

            Points.Add(pt.transform);
        }
    }
}
