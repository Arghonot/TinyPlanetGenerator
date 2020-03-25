using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public PlanetProfile profile;
    public bool UseMaps;
    public int AmountOfVertices;
    public GameObject water;

    public CustomPerlinGenerator generator;
    MeshFilter filter;
    MeshRenderer render;

    bool Draw = false;
    Vector3[] _normals;
    Vector3[] _vertices;
    List<Tuple<Vector3, Tuple<Vector3, Vector3>>> _editedNormals;
    List<Vector3> pos;
    public float length;

    #region UNITY

    private void Start()
    {
        if (!UseMaps)
        {
            Generate();
        }
    }

    #endregion

    #region REGENERATE

    public void Regenerate()
    {
        generator.Generate(profile);
        ReScale();

        if (UseMaps)
        {
            render.material = profile.material;

            // if lit
            if (render.material.shader.name == "Universal Render Pipeline/Lit")
            {
                //if (profile.name.Contains("Magma"))
                //{
                //    render.material.SetTexture("_EmissionMap", generator.InverseHeightMap);
                //}

                render.material.SetTexture("_BaseMap", generator.ColorMap);
            }
            // if unlit
            else
            {
                render.material.SetTexture("_MainTex", generator.ColorMap);
            }
        }

        if (profile.useWater)
        {
            water.SetActive(true);
        }
        else
        {
            water.SetActive(false);
        }
    }

    #endregion

    #region INIT

    public void Generate()
    {
        generator = GetComponent<CustomPerlinGenerator>();

        Mesh mesh = new Mesh();

        SetupVerticesAndUV(mesh);

        mesh.triangles = ProcessTriangles();
        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
        render = GetComponent<MeshRenderer>();

        //if (UseMaps)
        //{
        //    render.material.SetTexture("_BaseMap", generator.ColorMap);
        //}
    }

    #endregion

    #region Elevation

    void ReScale()
    {
        // We don't want to recalculate the elevation if there is none
        if (!UseMaps)
        {
            return;
        }

        Vector3[] vertices = new Vector3[
            ((AmountOfVertices + 1) * (AmountOfVertices + 1))];
        Vector3[] normals = new Vector3[vertices.Length];
        float latPadding = 180f / (float)AmountOfVertices;
        float lonPadding = 360f / (float)AmountOfVertices;

        for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        {
            for (int y = 0; y <= AmountOfVertices; y++, i++)
            {
                vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                    (((x * lonPadding) - 180f) * Mathf.Deg2Rad),
                    (((y * latPadding) - 90f) * Mathf.Deg2Rad),
                    profile.BaseElevation + (GetGrayScale(
                            generator.HeightMap,
                            x,
                            y) * profile.ElevationMultiplier));
            }
        }

        //for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        //{
        //    for (int y = 0; y <= AmountOfVertices; y++, i++)
        //    {
        //        normals[i] = getNormal(i, vertices);
        //    }
        //}

        filter.mesh.vertices = vertices;
        filter.mesh.RecalculateNormals();
        Vector3[] copied_normals = filter.mesh.normals;
        copied_normals = CalCulateNormals(copied_normals, vertices);
        //filter.mesh.RecalculateTangents();
        filter.mesh.normals = copied_normals;

        Draw = true;
        _vertices = vertices;
        _normals = filter.mesh.normals;
    }

    Vector3[] CalCulateNormals(Vector3[] normals, Vector3[] vertices)
    {
        pos = new List<Vector3>();
        _editedNormals = new List<Tuple<Vector3, Tuple<Vector3, Vector3>>>();

        for (int i = 0; i <= AmountOfVertices; i++)
        {
            int endindex = ((1 + AmountOfVertices) * (1 + AmountOfVertices)) - (AmountOfVertices) + i - 1;

            //Vector3 leftvector = vertices[endindex - 1] - vertices[endindex];
            //Vector3 rightvector = vertices[i + 1] - vertices[i];

            //Vector3 leftvector = vertices[endindex] - vertices[endindex - 1];
            //Vector3 rightvector = vertices[i] - vertices[i + 1];
            ////Vector3 newnormal = Vector3.Cross(leftvector, rightvector).normalized;
            //Vector3 newnormal = -(leftvector + rightvector).normalized;

            Vector3 leftvector = vertices[endindex] + vertices[endindex - 1];
            Vector3 rightvector = vertices[i] + vertices[i + 1];
            //Vector3 newnormal = Vector3.Cross(leftvector, rightvector).normalized;
            Vector3 newnormal = (leftvector + rightvector).normalized;

            if (newnormal.magnitude < 0)
                newnormal *= -1f;

            //print(i + " " + endindex);
            print(newnormal);
            normals[i] = normals[endindex] = newnormal;
            _editedNormals.Add(
                new Tuple<Vector3, Tuple<Vector3, Vector3>>(
                    vertices[i],
                    new Tuple<Vector3, Vector3>(
                        normals[i],
                        normals[endindex])));
        }

        print(_editedNormals.Count);

        // 2 loops
        //for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        //{
        //    int begin = i;
        //    for (int y = 0; y <= AmountOfVertices; y++, i++)
        //    {
        //        if (x == 0 || x == AmountOfVertices)
        //        {
        //            print(normals[begin]);
        //            Vector3 newnormal = (normals[begin] + normals[i - 1]).normalized;
        //            pos.Add(vertices[begin]);
        //            pos.Add(vertices[i - 1]);
        //            normals[begin] = newnormal;
        //            normals[i - 1] = normals[begin];

        //            print(begin + " " + (i - 1));
        //            print(normals[begin]);
        //        }
        //    }
        //}

        // 1 loop attempt
        //for (int i = 0; i <= AmountOfVertices; i++)
        //{
        //    int index = i * AmountOfVertices;

        //    normals[index] = (normals[index] + normals[index + AmountOfVertices]).normalized;
        //    normals[index + AmountOfVertices] = normals[index];

        //    print(index + "  " + (index + AmountOfVertices));
        //}

        return normals;
    }

    //Vector3 SurfaceNormalFromIndices(int Index, Vector3[] vertices)
    //{
    //    Vector3 normal = Vector3.zero;
    //    if (Index < AmountOfVertices + 1 || Index > (AmountOfVertices - 1) * AmountOfVertices)
    //    {
    //        return vertices[Index];
    //    }

    //    Vector3 SideA = vertices[Index + AmountOfVertices] - vertices[Index];
    //    Vector3 SideB = vertices[Index + AmountOfVertices - 1] - vertices[Index];
                
    //    Vector3 SideC = vertices[Index -1 ] - vertices[Index];
    //    Vector3 SideD = vertices[Index - AmountOfVertices - 1] - vertices[Index];
                
    //    Vector3 SideE = vertices[Index - AmountOfVertices] - vertices[Index];
    //    Vector3 SideF = vertices[Index - AmountOfVertices + 1] - vertices[Index];
                
    //    Vector3 SideG = vertices[Index + 1] - vertices[Index];
    //    Vector3 SideH = vertices[Index + AmountOfVertices + 1] - vertices[Index];

    //    Vector3 CrossedNormalA = Vector3.Cross(SideB, SideA);
    //    Vector3 CrossedNormalB = Vector3.Cross(SideC, SideD);
    //    Vector3 CrossedNormalC = Vector3.Cross(SideE, SideF);
    //    Vector3 CrossedNormalD = Vector3.Cross(SideG, SideH);

    //    Vector3 DoubleCrossedNormalA = Vector3.Cross(CrossedNormalA, CrossedNormalB);
    //    Vector3 DoubleCrossedNormalB = Vector3.Cross(CrossedNormalC, CrossedNormalD);

    //    return CrossedNormalA;
    //    return Vector3.Cross(DoubleCrossedNormalA, DoubleCrossedNormalB);

    //    //normal += vertices[Index - 1];
    //    //normal += vertices[Index + 1];

    //    //normal += vertices[Index - AmountOfVertices - 1];
    //    //normal += vertices[Index - AmountOfVertices];
    //    //normal += vertices[Index - AmountOfVertices + 1];

    //    //normal += vertices[Index + AmountOfVertices - 1];
    //    //normal += vertices[Index + AmountOfVertices];
    //    //normal += vertices[Index + AmountOfVertices + 1];

    //    print("CALCULATED NORMAL");
    //    return normal;
    //}

    //Vector3[] CalCulateNormals(Vector3[] vertices, int[] triangles)
    //{
    //    Vector3[] vertexNormals = new Vector3[vertices.Length];
    //    int triangleCount = triangles.Length / 3;

    //    for (int i = 0; i < triangleCount; i++)
    //    {
    //        int normalTriangleIndex = i * 3;

    //        int vertexIndexA = triangles[normalTriangleIndex];
    //        int vertexIndexB = triangles[normalTriangleIndex + 1];
    //        int vertexIndexC = triangles[normalTriangleIndex + 2];

    //        print(vertexIndexA);

    //        //if (normalTriangleIndex % (AmountOfVertices * 2) == 0)
    //        //{
    //        //    print("WLESH : " + normalTriangleIndex
    //        //        + "    " + vertexIndexA
    //        //        + "    " + vertexIndexB
    //        //        + "    " + vertexIndexC);
    //        //}

    //        //print(i + " " + vertexIndexA + "   " + vertexIndexB + "   " + vertexIndexC);

    //        Vector3 triangleNormal = SurfaceNormalFromIndices(
    //            vertexIndexA,
    //            vertexIndexB,
    //            vertexIndexC,
    //            vertices);
    //        vertexNormals[vertexIndexA] += triangleNormal;
    //        vertexNormals[vertexIndexB] += triangleNormal;
    //        vertexNormals[vertexIndexC] += triangleNormal;
    //    }

    //    for (int i = 0; i < vertexNormals.Length; i++)
    //    {
    //        vertexNormals[i].Normalize();
    //    }

    //    return vertexNormals;
    //}

    //Vector3 SurfaceNormalFromIndices(int IndexA, int IndexB, int IndexC, Vector3[] vertices)
    //{
    //    Vector3 pointA = vertices[IndexA];
    //    Vector3 pointB = vertices[IndexB];
    //    Vector3 pointC = vertices[IndexC];

    //    //print(IndexA + "    " + IndexB + "  " + IndexC);
    //    // if begin of the mesh

    //    Vector3 sideAB = pointB - pointA;
    //    Vector3 sideAC = pointC - pointA;

    //    return Vector3.Cross(sideAB, sideAC).normalized;
    //}

    float GetGrayScale(Texture2D tex, int x, int y)
    {
        if (x == AmountOfVertices && y == AmountOfVertices)
        {
            return tex.GetPixel(tex.width - 1, tex.height - 1).grayscale;
        }

        return tex.GetPixel(
            (int)((float)tex.width * ((float)x / (float)AmountOfVertices)),
            (int)((float)tex.height * ((float)y / (float)AmountOfVertices))).grayscale;
    }

    #endregion

    #region MeshGeneration

    void SetupVerticesAndUV(Mesh mesh)
    {
        Vector3[] vertices = new Vector3[
            ((AmountOfVertices + 1) * (AmountOfVertices + 1))];
        Vector2[] uv = new Vector2[vertices.Length];
        float latPadding = 180f / (float)AmountOfVertices;
        float lonPadding = 360f / (float)AmountOfVertices;

        for (int x = 0, i = 0; x <= AmountOfVertices; x++)
        {
            for (int y = 0; y <= AmountOfVertices; y++, i++)
            {
                    vertices[i] = CoordinatesProjector.InverseMercatorProjector(
                        (((x * lonPadding) - 180f) * Mathf.Deg2Rad),
                        (((y * latPadding) - 90f) * Mathf.Deg2Rad),
                        .96f);

                //print("[X][DEG]" + x + " " + (x * lonPadding) + "    [Y][DEG]" + y + " " + (y * latPadding));

                uv[i] = new Vector2(
                    (float)x / (float)AmountOfVertices,
                    (float)y / (float)AmountOfVertices);
            }
        }

        mesh.RecalculateNormals();
        mesh.vertices = vertices;
        mesh.uv = uv;
    }

    int[] ProcessTriangles()
    {
        int[] triangles = new int[((AmountOfVertices * AmountOfVertices) + AmountOfVertices) * 6];

        for (int ti = 0, vi = 0, y = 0; y < AmountOfVertices; y++, vi++)
        {
            for (int x = 0; x < AmountOfVertices; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + AmountOfVertices + 1;
                triangles[ti + 5] = vi + AmountOfVertices + 2;
            }
        }

        return triangles;
    }

    #endregion

    #region DEBUG

    private void OnDrawGizmos()
    {
        if (!Draw)
        {
            return;
        }

        //Gizmos.DrawLine(
        //    transform.position + _vertices[4],
        //    transform.position + _vertices[4] + (_normals[4] * length));
        //Gizmos.DrawLine(
        //    transform.position + _vertices[34],
        //    transform.position + _vertices[34] + (_vertices[34] * length));

        for (int i = 0; i < _editedNormals.Count; i++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position + _editedNormals[i].Item1,
                transform.position + _editedNormals[i].Item1 + (_editedNormals[i].Item2.Item1 * length));

            Gizmos.color = Color.white;

            Gizmos.DrawLine(
                transform.position + _editedNormals[i].Item1,
                transform.position + _editedNormals[i].Item1 + (_editedNormals[i].Item2.Item2 * -length));
        }

        //for (int i = 0; i < _vertices.Length; i++)
        //{
        //   Gizmos.color = Color.green;

        //    Gizmos.DrawLine(
        //        transform.position + _vertices[i],
        //        transform.position + _vertices[i] + ((_normals[i] * length) / 2f));
        //    //(new Ray(_vertices[i], _normals[i] * length));
        //}

        //for (int i = 0; i < pos.Count; i++)
        //{
        //    //print("sjhsdjh");
        //    Gizmos.DrawSphere(transform.position + pos[i], length * 10f);
        //}
    }

    #endregion
}
