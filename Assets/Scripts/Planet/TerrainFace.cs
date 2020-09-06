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

    Gradient _grad;

    #region Face Management
    /// <summary>
    /// Initialize all the data that will be used by this face in order to
    /// compute all the vertices position + elevation.
    /// </summary>
    /// <param name="mesh">The mesh that will store all the datas.</param>
    /// <param name="resolution">The amount of vertice per length. vertices.length = (resolution * resolution).</param>
    /// <param name="localUp">The direction this face is facing, planet relative.</param>
    public void InitTerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    /// <summary>
    /// Create the face's mesh.
    /// MUST be called after a InitTerrainFace.
    /// </summary>
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

    /// <summary>
    /// Sample noise's pixels to compute every vertices elevation and vertex's color color.
    /// </summary>
    /// <param name="noise">The texture representing the planet.</param>
    /// <param name="baseElevation">The minimum radius the planet can be.</param>
    /// <param name="meanElevation">The maximum elevation the planet can have (base + mean).</param>
    /// <param name="grad">The color gradient the texture is based on.</param>
    public void ElevateMesh(Texture2D noise, float baseElevation, float meanElevation)
    {
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
                var invertedln = Mathf.Abs(ln - 360f) + 90f;

                intensity = baseElevation + (GetGrayScale(invertedln, lat) * meanElevation);// ln, la are in [-180;180] - [-90;90] interval here
                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    ln * Mathf.Deg2Rad,
                    lat * Mathf.Deg2Rad,
                    intensity);

                if (x < resolution && y == 0)
                {

                }

                colors[i] = GetColor(invertedln, lat); // ln, la are in [-180;180] - [-90;90] interval here
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        mesh.normals = CalculateNormals();
    }

    #endregion

    #region Normal Computation

    /// <summary>
    /// Return the normal of 3 points (AB AC).
    /// </summary>
    /// <param name="pointA">The point that is at the origin of both vectors.</param>
    /// <param name="pointB">The point B.</param>
    /// <param name="pointC">The point C.</param>
    /// <returns>The normalized normal of the 3 points.</returns>
    Vector3 SurfaceNormalFromIndices(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    /// <summary>
    /// Calculate the normals of the face.
    /// </summary>
    /// <returns>The computed normals.</returns>
    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = mesh.normals;
        int triangleCount = mesh.triangles.Length / 3;

        //for (int i = resolution; i < (resolution * resolution) - resolution; i++)
        //{
        //    if (i % resolution > 0 && (i + 1) % resolution > 0)
        //    {
        //        var vector1 = SurfaceNormalFromIndices(
        //            vertexNormals[i],
        //            vertexNormals[i - resolution - 1],
        //            vertexNormals[i - resolution]);
        //        var vector2 = SurfaceNormalFromIndices(
        //            vertexNormals[i],
        //            vertexNormals[i - resolution + 1],
        //            vertexNormals[i + 1]);
        //        var vector3 = SurfaceNormalFromIndices(
        //            vertexNormals[i],
        //            vertexNormals[i + resolution + 1],
        //            vertexNormals[i + resolution]);
        //        var vector4 = SurfaceNormalFromIndices(
        //            vertexNormals[i],
        //            vertexNormals[i + resolution - 1],
        //            vertexNormals[i - 1]);
        //        vertexNormals[i] = -(vector1 + vector2 + vector3 + vector4);
        //    }
        //}

        for (int i = 0; i < resolution; i++)
        {
            vertexNormals[i] = GetNormal(i);

            if (i > 0 && i < resolution - 1)
            {
                vertexNormals[resolution * i] =
                    GetNormal(i * resolution);

                vertexNormals[(resolution * i) + resolution - 1] =
                    GetNormal((resolution * i) + resolution - 1);
            }

            vertexNormals[(resolution * resolution) - resolution + i] =
                GetNormal((resolution * resolution) - resolution + i);
        }

        //for (int i = 0; i < vertexNormals.Length; i++)
        //{
        //    //vertexNormals[i] = vertexNormals[i].normalized;
        //    vertexNormals[i].Normalize();
        //}

        return vertexNormals;
    }

    /// <summary>
    /// Compute the normal for a vertice according to it's longitude latitude position.
    /// </summary>
    /// <param name="i">The vertice index.</param>
    /// <returns>The vertice's normal.</returns>
    Vector3 GetNormal(int i)
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

        return (-(One + Two + Three + Four)).normalized;
    }

    /// <summary>
    /// Get all 8 position on a grid sourrounding the I vertice.
    /// </summary>
    /// <param name="i">The index of the source vertice.</param>
    /// <returns>All 8 positions.</returns>
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

    #endregion

    #region Misc

    /// <summary>
    /// Get a position using a 3d position and x and y offsets.
    /// </summary>
    /// <param name="pos">The origin position.</param>
    /// <param name="XOffset">the magnitude of the longitude offset.</param>
    /// <param name="YOffset">the magnitude of the latitude offset.</param>
    /// <returns>The 3D position of the offseted point.</returns>
    Vector3 GetPosition(Vector3 pos, int XOffset, int YOffset)
    {
        Vector2 posLnLat = CoordinatesProjector.GetLnLatFromPosition(pos);
        float lnOffset = 360f / ((float)resolution * 4f);
        float latOffset = 180f / ((float)resolution * 4f) * 2f;
        Vector2 newpos = new Vector2(
            posLnLat.x + (lnOffset * (XOffset / 2f)),
            posLnLat.y + (latOffset * (YOffset / 2f)));
        float MapLn = Mathf.Abs(newpos.x - 360f) + 90f;
        float elevation = BaseElevation + (GetGrayScale(MapLn, newpos.y) * MeanElevation);

        Vector3 finalpos = CoordinatesProjector.InverseMercatorProjector(
                    (newpos.x) * Mathf.Deg2Rad,
                    (newpos.y) * Mathf.Deg2Rad,
                    elevation);

        return finalpos;
    }

    /// <summary>
    /// Get the color of a position from the face's texture.
    /// </summary>
    /// <param name="ln">The longitude position.</param>
    /// <param name="la">The latitude position.</param>
    /// <returns>The color sampled at said position.</returns>
    Color GetColor(float ln, float la)
    {
        ln += 180f;
        la += 90f;

        return tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f)));
    }

    /// <summary>
    /// Get the elevation of a position from the face's texture.
    /// </summary>
    /// <param name="ln">The longitude position.</param>
    /// <param name="la">The latitude position.</param>
    /// <returns>The height sampled at said position.</returns>
    float GetGrayScale(float ln, float la)
    {
        ln += 180f;
        la += 90f;
        Color col = tex.GetPixel(
            (int)((float)tex.width * (ln / 360f)),
            (int)((float)tex.height * (la / 180f)));
        float redval = (float)(col.r * 0.33);
        float greenval = (float)(col.g * 0.66);
        float blueval = (float)(col.b);

        return redval + greenval + blueval;

        //return tex.GetPixel(
        //     (int)((float)tex.width * (ln / 360f)),
        //     (int)((float)tex.height * (la / 180f))).grayscale;
    }

    #endregion
}