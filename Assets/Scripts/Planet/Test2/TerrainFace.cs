using LibNoise;
using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace : MonoBehaviour
{
    public Mesh mesh;
    public  int resolution;
    public  Vector3 localUp;
    public  Vector3 axisA;
    public  Vector3 axisB;

    public  float BaseElevation;
    public  float MeanElevation;

    public  Vector3[] vertices;
    public  Texture2D tex;

    public void InitTerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        //Debug.Log(mesh.name + " " + localUp + "   " + axisA + "   " + axisB);
    }

    public void ConstructMesh()
    {
        // TODO put this in planettest.cs
        // we make sure resolution becomes pair
        // so a 3x3 stays a 3x3 visually but become a 4x4
        if (resolution % 2 != 0)
        {
            resolution += 1;
        }

        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(resolution - 1) * (resolution) * 6];
        int triIndex = 0;
        float ln = 0f;
        float lat = 0f;
        float lnOffset = 360f / (float)((resolution - 1) * 4);
        float latOffset = 180f / (float)((resolution - 1) * 4);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;

                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;

                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                ln = CoordinatesProjector.CartesianToLon(pointOnUnitSphere);
                lat = CoordinatesProjector.CartesianToLat(pointOnUnitSphere);

                vertices[i] = pointOnUnitSphere;
                uvs[i] = percent;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void ElevateMesh(Texture2D noise, float baseElevation, float meanElevation, Gradient grad = null)
    {
        if (grad == null) return;

        BaseElevation = baseElevation;
        MeanElevation = meanElevation;
        tex = noise;
        vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];
        Color[] colors = new Color[normals.Length];

        float ln = 0f;
        float lat = 0f;
        float intensity = 0f;

        for (int x = 0, i = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++, i++)
            {
                ln = CoordinatesProjector.CartesianToLon(vertices[i].normalized);
                lat = CoordinatesProjector.CartesianToLat(vertices[i].normalized);

                intensity = baseElevation + (GetGrayScale(ln, lat) * meanElevation);// ln, la are in [-180;180] - [-90;90] interval here
                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    ln * Mathf.Deg2Rad,
                    lat * Mathf.Deg2Rad,
                    intensity);

                if (x < resolution && y == 0)
                {

                }

                colors[i] = GetColor(ln, lat); // ln, la are in [-180;180] - [-90;90] interval here
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        mesh.normals = CalculateNormals();
    }

    void CreateMarks()
    {
        for (int x = 0, i = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++, i++)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = i.ToString() + "___" + x.ToString() + "_" + y.ToString();
                sphere.transform.position = mesh.vertices[i];
                sphere.transform.localScale = Vector3.one * 0.01f;
            }
        }
    }

    Vector3 SurfaceNormalFromIndices(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = mesh.normals;
        int triangleCount = mesh.triangles.Length / 3;
        float lnOffset = (360f / (float)((resolution - 1)) * .75f) / 2f;
        float latOffset = 180f / (float)((resolution - 1)) * .75f;


        for (int i = 0; i < resolution; i++)
        {
            // red
            //vertexNormals[i] += GetNormal(i, 1, -1);
            vertexNormals[i] = GetNormal(i);
            if (i > 0 && i < resolution - 1)
            {
                // do begin and last from line
                // green
                //vertexNormals[resolution * i] +=
                vertexNormals[resolution * i] = GetNormal(i * resolution);

                //blue
                //vertexNormals[resolution * i + resolution - 1] +=
                vertexNormals[(resolution * i) + resolution - 1] = GetNormal((resolution * i) + resolution - 1, 3);
            }

            ////// black
            //vertexNormals[(resolution * resolution) - resolution + i] +=
            vertexNormals[(resolution * resolution) - resolution + i] = GetNormal((resolution * resolution) - resolution + i, 4);
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    Vector3 GetNormal(int i, int type = 0)
    {
        Vector3 vertice = mesh.vertices[i];
        Vector3[] Vectors = SetupPositions(i);

        Vector3 One = SurfaceNormalFromIndices(
            vertice,
            Vectors[0],
            Vectors[1]);
        Vector3 Two = SurfaceNormalFromIndices(
            vertice,
            Vectors[2],
            Vectors[4]);
        Vector3 Three = SurfaceNormalFromIndices(
            vertice,
            Vectors[7],
            Vectors[6]);
        Vector3 Four = SurfaceNormalFromIndices(
            vertice,
            Vectors[5],
            Vectors[3]);

        //if (gameObject.name.Contains("5") ||
        //    gameObject.name.Contains("2"))
        //{
        //    if (i == 39)
        //    {
        //        Debug.Log(gameObject.name + "   " + vertice);
        //    }
        //}

        return -(One + Two + Three + Four);//+ Five + Six);
    }

    Vector3[] SetupPositions(int i)
    {
        Vector3[] Vectors = new Vector3[8];

        Vectors[0] = GetPosition(
            mesh.vertices[i],
            -1,
            1);
        Vectors[1] = GetPosition(
             mesh.vertices[i],
             0,
             1);
        Vectors[2] = GetPosition(
            mesh.vertices[i],
            1,
            1);
        Vectors[3] = GetPosition(
             mesh.vertices[i],
             -1,
             0);
        Vectors[4] = GetPosition(
            mesh.vertices[i],
            1,
            0);
        Vectors[5] = GetPosition(
             mesh.vertices[i],
             -1,
             -1);
        Vectors[6] = GetPosition(
            mesh.vertices[i],
            0,
            -1);
        Vectors[7] = GetPosition(
             mesh.vertices[i],
             1,
             -1);

        return Vectors;
    }

    Vector3 GetPosition(Vector3 pos, int XOffset, int YOffset)
    {
        Vector2 posLnLat = CoordinatesProjector.GetLnLatFromPosition(pos);
        float lnOffset = 360f / ((float)resolution * 4f);
        float latOffset = 180f / ((float)resolution * 4f) * 2f;
        Vector2 newpos = new Vector2(
            posLnLat.x + (lnOffset * (XOffset / 2f)),
            posLnLat.y + (latOffset * (YOffset / 2f)));
        float elevation = BaseElevation + (GetGrayScale(newpos.x, newpos.y) * MeanElevation);

        Vector3 finalpos = CoordinatesProjector.InverseMercatorProjector(
                    (newpos.x) * Mathf.Deg2Rad,
                    (newpos.y) * Mathf.Deg2Rad,
                    elevation);

        return finalpos;
    }

    Color GetColor(float ln, float la)
    {
        ln += 180f;
        la += 90f;

        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f)));
    }

    float GetGrayScale(float ln, float la)
    {
        ln += 180f;
        la += 90f;

        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f))).grayscale;
    }
}