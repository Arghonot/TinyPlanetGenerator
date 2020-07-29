using LibNoise;
using LibNoise.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    public Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    float BaseElevation;
    float MeanElevation;

    Vector3[] vertices;
    Texture2D tex;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
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

                intensity = baseElevation + (GetGrayScale(ln + 180f, lat + 90f) * meanElevation);
                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    ln * Mathf.Deg2Rad,
                    lat * Mathf.Deg2Rad,
                    intensity);

                if (x < resolution && y == 0)
                {

                }
                //if (x >)

                colors[i] = GetColor(ln + 180f, lat + 90f); //grad.Evaluate(intensity);
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        mesh.normals = CalculateNormals();
    }

    Vector3 SurfaceNormalFromIndices(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = mesh.normals; // new Vector3[vertices.Length];
        int triangleCount = mesh.triangles.Length / 3;
        float lnOffset = 360f / (float)((resolution - 1) * 4);
        float latOffset = 180f / (float)((resolution - 1) * 4);

        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    vertexNormals[i] = vertices[i].normalized;
        //}
        //return vertexNormals;

        //for (int i = 0; i < triangleCount; i++)
        //{
        //    int normalTriangleIndex = i * 3;
        //    int vertexIndexA = mesh.triangles[normalTriangleIndex];
        //    int vertexIndexB = mesh.triangles[normalTriangleIndex + 1];
        //    int vertexIndexC = mesh.triangles[normalTriangleIndex + 2];

        //    Vector3 triangleNormal = SurfaceNormalFromIndices(
        //        vertices[vertexIndexA],
        //        vertices[vertexIndexB],
        //        vertices[vertexIndexC]);

        //    vertexNormals[vertexIndexA] += triangleNormal;
        //    vertexNormals[vertexIndexB] += triangleNormal;
        //    vertexNormals[vertexIndexC] += triangleNormal;
        //}

        //if (mesh.name.Contains("2"))
        //{
        //    Debug.Log(resolution);

        //    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    sphere.transform.localScale = Vector3.one * .1f;
        //    sphere.transform.position = vertices[2];
        //    sphere.name = "vertice2"; 
        //}
        for (int i = 0; i < resolution; i++)
        {
            // red
            vertexNormals[i] += GetNormal(i, 1, -1);
            vertexNormals[i] += GetNormal2(i, -1, -1);
            vertexNormals[i] += GetNormal2(i, 1, -1);
            vertexNormals[i] += GetNormal(i, -1, -1);

            if (i > 0 && i < resolution - 1)
            {
                // do begin and last from line
                // green
                vertexNormals[resolution * i] +=
                    GetNormal(resolution * i, -1, -1);
                vertexNormals[resolution * i] +=
                    GetNormal2(resolution * i, -1, -1);
                vertexNormals[resolution * i] +=
                    GetNormal(resolution * i, -1, 1);
                vertexNormals[resolution * i] +=
                    GetNormal2(resolution * i, -1, 1);
                //blue
                vertexNormals[resolution * i + resolution - 1] +=
                    GetNormal2(resolution * i + resolution - 1, 1, -1);
                vertexNormals[resolution * i + resolution - 1] +=
                    GetNormal(resolution * i + resolution - 1, 1, -1);
                vertexNormals[resolution * i + resolution - 1] +=
                    GetNormal(resolution * i + resolution - 1, 1, 1);
                vertexNormals[resolution * i + resolution - 1] +=
                    GetNormal2(resolution * i + resolution - 1, 1, 1);
                //GetNormal(vertexNormals[resolution * i + resolution]); 
            }

            ////// black
            vertexNormals[(resolution * resolution) - resolution + i] +=
                GetNormal((resolution * resolution) - resolution + i, 1, 1);
            vertexNormals[(resolution * resolution) - resolution + i] +=
                GetNormal2((resolution * resolution) - resolution + i, 1, 1);
            vertexNormals[(resolution * resolution) - resolution + i] +=
                GetNormal2((resolution * resolution) - resolution + i, -1, 1);
            vertexNormals[(resolution * resolution) - resolution + i] +=
                GetNormal((resolution * resolution) - resolution + i, -1, 1);
            //GetNormal(vertexNormals[(resolution * resolution) - resolution + i]);
        }
        ////}

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }


    public Vector3 GetNormal2(int index, int xoffset, int yoffset, bool debug = false)
    {
        int x = index % (resolution);
        int y = index / (resolution);

        float step = 1f / ((float)resolution - 1f);

        Vector2 verticeAPercentage = GetPercentage(x, y);
        Vector3 posA = vertices[index];

        Vector3 posB = GetSpherifiedPositionFromXY(x, y + yoffset);
        Vector3 posC = GetSpherifiedPositionFromXY(x + xoffset, y + yoffset);

        //if (mesh.name.Contains("2"))
        //{
        //    if (debug)
        //    {
        //        Debug.Log(index);
        //    }

        //    if (index == 60 && debug)
        //    {
        //        Debug.Log("NORMAL2 : " +
        //            GetPercentage(x + xoffset, y) +
        //            "   " +
        //            GetPercentage(x + xoffset, y + yoffset));
        //        createSphere("A", posA);
        //        createSphere("B", posB);
        //        createSphere("C", posC);
        //    }
        //}

        return SurfaceNormalFromIndices(posA, posB, posC);
    }
    public Vector3 GetNormal(int index, int xoffset, int yoffset, bool debug = false)
    {
        int x = index % (resolution);
        int y = index / (resolution);

        float step = 1f / ((float)resolution - 1f);

        Vector2 verticeAPercentage = GetPercentage(x, y);
        Vector3 posA = vertices[index];

        Vector3 posB = GetSpherifiedPositionFromXY(x + xoffset, y);
        Vector3 posC = GetSpherifiedPositionFromXY(x + xoffset, y + yoffset);

        if (mesh.name.Contains("2"))
        {
            if (debug)
            {
                Debug.Log(index); 
            }

            if (index == 60 && debug)
            {
                Debug.Log(
                    GetPercentage(x + xoffset, y) +
                    "   " +
                    GetPercentage(x + xoffset, y + yoffset));
                createSphere("A", posA);
                createSphere("B", posB);
                createSphere("C", posC);
            }
        }

        return SurfaceNormalFromIndices(posA, posB, posC);
    }

    public void createSphere(string name, Vector3 pos)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphere.name = name;
        sphere.transform.localScale = Vector3.one * .1f;
        sphere.transform.position = pos;
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

    public Vector2 GetPercentage(int x, int y)
    {
        return new Vector2(x, y) / (resolution - 1);
    }

    public Vector3 GetVerticePosition(Vector2 percent)
    {
        return (localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB).normalized;
    }

    public Vector3 GetNormal(Vector3 currentpos)
    {
        float currentLn = CoordinatesProjector.CartesianToLon(currentpos);
        float currentLat = CoordinatesProjector.CartesianToLat(currentpos);
        float lnOffset = 360f / (float)((resolution - 1) * 4);
        float latOffset = 180f / (float)((resolution - 1) * 4);
        float newln = currentLn + lnOffset;
        float newlat = currentLat + latOffset;

        Vector3 PointB = CoordinatesProjector.InverseMercatorProjector(
            newln,
            newlat,
            1 + GetGrayScale(
                newln,
                newlat));
        Vector3 PointC = CoordinatesProjector.InverseMercatorProjector(
             newln,
             newlat,
             1 + GetGrayScale(
                 newln,
                 currentLat));

        return SurfaceNormalFromIndices(currentpos, PointB, PointC);
    }

    float GetGrayScale(Vector2 pos)
    {
        return GetGrayScale(pos.x, pos.y);
    }

    Color GetColor(float ln, float la)
    {
        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f)));
    }

    float GetGrayScale(float ln, float la)
    {
        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f))).grayscale;
    }
}