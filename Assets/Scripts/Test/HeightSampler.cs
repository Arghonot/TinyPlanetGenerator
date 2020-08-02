using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//
//  1   2   3
//  5   X   5
//  6   7   8
//



[ExecuteInEditMode]
public class HeightSampler : MonoBehaviour
{
    public bool Run;
    public Vector2 lnlatoffset;
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

    Vector2 OldLnLatOffset;

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
                if (Points == null || Points.Count == 0 || Points[0] == null)
                {
                    Init();
                }

                SampleSurroundingPositions();
            }

            Run = false;
        }
        if (lnlatoffset != OldLnLatOffset)
        {
            OldLnLatOffset = lnlatoffset;
            CustomOffset();
        }
    }

    void SampleSurroundingPositions()
    {
        ResizePoints();

        Points.ForEach(x => x.transform.position = 
            filter.sharedMesh.vertices[(X * Resolution) + Y]);

        Vector3 pos = filter.sharedMesh.vertices[(X * Resolution) + Y];//GetSpherifiedPositionFromXY(X, Y);

        // 1
        Points[0].transform.position = getpos(pos, -1, 1);

        // 2
        Points[1].transform.position = getpos(pos, 0, 1);

        // 3
        Points[2].transform.position = getpos(pos, 1, 1);

        // 4
        Points[3].transform.position = getpos(pos, -1, 0);

        // 5
        Points[4].transform.position = getpos(pos, 1, 0);

        // 6
        Points[5].transform.position = getpos(pos, -1, -1);

        // 7
        Points[6].transform.position = getpos(pos, 0, -1);

        // 8
        Points[7].transform.position = getpos(pos, 1, -1);

        //Debug.Log(CoordinatesProjector.GetLnLatFromPosition(Points[0].transform.position));
        //Debug.Log(CoordinatesProjector.GetLnLatFromPosition(Points[1].transform.position));

    }

    void CustomOffset()
    {
        Debug.Log("CustomOffset");
        Vector3 pos = filter.sharedMesh.vertices[(X * Resolution) + Y];
        Vector2 posLnLat;// = CoordinatesProjector.GetLnLatFromPosition(pos);

        //posLnLat = new Vector2(posLnLat.x, posLnLat.y);

        posLnLat = Vector2.zero;
        Vector2 newpos = new Vector2(
            posLnLat.x + (lnlatoffset.x),
            posLnLat.y + (lnlatoffset.y));

        Vector3 finalpos = CoordinatesProjector.InverseMercatorProjector(
                    (newpos.x) * Mathf.Deg2Rad,
                    (newpos.y) * Mathf.Deg2Rad,
                    .5f);

        Points[1].transform.position = finalpos;
    }

    Vector3 getpos(Vector3 pos, int XOffset, int YOffset)
    {
        Vector2 posLnLat = CoordinatesProjector.GetLnLatFromPosition(pos);
        //posLnLat = new Vector2(posLnLat.x, posLnLat.y);
        float lnOffset = 360f / ((float)Resolution * 4f);
        float latOffset = 180f / ((float)Resolution * 4f) * 2f;
        Vector2 newpos = new Vector2(
            posLnLat.x + (lnOffset * XOffset),
            posLnLat.y + (latOffset * YOffset));
        float elevation = BaseElevation + (GetGrayScale(newpos.x, newpos.y) * MeanElevation);

        Vector3 finalpos = CoordinatesProjector.InverseMercatorProjector(
                    (newpos.x) * Mathf.Deg2Rad,
                    (newpos.y) * Mathf.Deg2Rad,
                    elevation);

        Debug.Log("getpos[posLnLat][lnOffset][latOffset][newpos][elevation][finalpos] : " +
            posLnLat.ToString() + " | " + lnOffset + " | " + latOffset + " | " + newpos.ToString() +
            " | " + elevation + " | " + finalpos.ToString());

        return finalpos;
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
        ln += 180f;
        la += 90f;
        int xpos = (int)((float)tex.width * (ln / 360f));
        int ypos = (int)((float)tex.height * (la / 180f));

        float greyscale = tex.GetPixel(
                xpos,
                ypos).grayscale;

        Debug.Log("GetGrayScale[xpos][ypos][greyscale] : " +
            xpos + " | " + ypos + " | " + greyscale.ToString());

        return greyscale;
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

        var face = GetComponent<TerrainFace>();

        if (face == null)
        {
            return;
        }

        Resolution = face.resolution;
        localUp = face.localUp;
        axisA = face.axisA;
        axisB = face.axisB;
        BaseElevation = face.BaseElevation;
        MeanElevation = face.MeanElevation;
        tex = face.tex;
    }
}
