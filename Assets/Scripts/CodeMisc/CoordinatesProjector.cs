using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordinatesProjector {

    // Project mercator from 2d coordinates (map : longitude / latitude)
    public static Vector3   InverseMercatorProjector(float longitude, float latitude, float radius)
    {
        return new Vector3(
            radius * Mathf.Cos(latitude) * Mathf.Sin(longitude),
            radius * Mathf.Sin(latitude),
            radius * Mathf.Cos(latitude) * Mathf.Cos(longitude));
    }

    public static float CartesianToRadius(Vector3 pos)
    {
        return Mathf.Sqrt(
            (pos.x * pos.x) +
            (pos.y * pos.y) +
            (pos.z * pos.z));
    }


    /// <summary>
    /// To be inputed into InverseMercatorProjector
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static float CartesianToLat(Vector3 pos)
    {
        return Mathf.Asin(pos.z / CartesianToRadius(pos)) * Mathf.Rad2Deg;
        //return Mathf.Atan(pos.y / pos.x);
    }

    public static float CartesianToLon(Vector3 pos)
    {
        return Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        //return Mathf.Atan(Mathf.Sqrt((pos.x * pos.x) + (pos.y * pos.y)) / pos.z);
    }

    public static float GetSimpleLatitude(int y, int meshsize)
    {
        float percentage = (float)y / meshsize;
        float uncenteredlatitude = percentage * 180.0f;
        return uncenteredlatitude;
    }

    public static float GetSimpleLongitude(int x, int meshsize)
    {
        float percentage = (float)x / meshsize;
        float uncenteredlatitude = percentage * 360.0f;

        return uncenteredlatitude;
    }

    // take it from between [0;max] to [-90;90], [-90;0]U[0;90] will not work with our inverse mercator projection
    public static float GetLatitude(int y, int meshsize)
    {
        float percentage = (float)y / meshsize;
        float uncenteredlatitude = percentage * 180.0f;
        return uncenteredlatitude - 90.0f;
    }

    // take it from between [0;max] to [0;360], [-180;0]U[0;180] will not work with our inverse mercator projection
    public static float GetLongitude(int x, int meshsize)
    {
        float percentage = (float)x / meshsize;
        float uncenteredlatitude = percentage * 360.0f;

        return 360.0f - uncenteredlatitude;
    }

    // return latitude relative to a position for a certain chunck
    public static float GetLatitudeFromPositions(int y, int meshsize, float beginCoordinate, float lengthCoordinates)
    {
        float percentage = (float)y / meshsize;
        float uncenteredlatitude = percentage * lengthCoordinates;
        return (beginCoordinate + lengthCoordinates) - uncenteredlatitude;
    }

    // return longitude relative to a position for a certain chunck
    public static float GetLongitudeFromPositions(int x, int meshsize, float beginCoordinate, float lengthCoordinates)
    {
        float percentage = (float)x / meshsize;
        float uncenteredlatitude = percentage * lengthCoordinates;

        return beginCoordinate + uncenteredlatitude;
    }

}
