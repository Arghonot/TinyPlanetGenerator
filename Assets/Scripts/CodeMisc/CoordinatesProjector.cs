using UnityEngine;

public static class CoordinatesProjector
{
    /// <summary>
    /// Take ln lat radius 2d positions and return a 3d cartesian position.
    /// </summary>
    /// <param name="longitude">The longitude variable in radian ([0;6.28319]rad, [0;360]deg).</param>
    /// <param name="latitude">The latitude variable in radian ([0;3.14159]rad, [0;180]deg).</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <returns></returns>
    public static Vector3   InverseMercatorProjector(float longitude, float latitude, float radius)
    {
        return new Vector3(
            radius * Mathf.Cos(latitude) * Mathf.Sin(longitude),
            radius * Mathf.Sin(latitude),
            radius * Mathf.Cos(latitude) * Mathf.Cos(longitude));
    }

    /// <summary>
    /// Return the radius of a sphere that extend from V3.zero to pos.
    /// </summary>
    /// <param name="point">A point at the surface of the sphere.</param>
    /// <returns>The sphere radius.</returns>
    public static float CartesianToRadius(Vector3 point)
    {
        return Mathf.Sqrt(
            (point.x * point.x) +
            (point.y * point.y) +
            (point.z * point.z));
    }

    /// <summary>
    /// Take a 3d world position and scale it to a 2d ln[-180f;180f] lat[-90f; 90f] spherical coord in degrees.
    /// the radius is lost in the process.
    /// </summary>
    /// <param name="position">The 3d cartesian position.</param>
    /// <returns>The spherical coord (deg) based on the 3d pos.</returns>
    public static Vector2 GetLnLatFromPosition(Vector3 position)
    {
        return new Vector2(
            CartesianToLon(position),
            CartesianToLat(position));
    }

    /// <summary>
    /// Take a point at the surface of a sphere with a V3.zero origin and return the point's latitude in degrees.
    /// We consider Vector3.Up to be equal to 180 of latitude.
    /// </summary>
    /// <param name="point">The point at the surface of the sphere.</param>
    /// <returns>The latitude (deg) of the point.</returns>
    public static float CartesianToLat(Vector3 point)
    {
        //return Mathf.Asin(point.y / CartesianToRadius(point)) * Mathf.Rad2Deg;
        return Mathf.Asin(point.y / CartesianToRadius(point)) * Mathf.Rad2Deg;
        //return Mathf.Asin(point.z / CartesianToRadius(point)) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Take a point at the surface of a sphere with a V3.zero origin and return the point's longitude in degrees.
    /// We consider Vector3.forward to be equal to 0 or 360f of longitude.
    /// </summary>
    /// <param name="point">The point at the surface of the sphere.</param>
    /// <returns>The longitude of the point (deg).</returns>
    public static float CartesianToLon(Vector3 point)
    {
        return Mathf.Atan2(point.x, point.z) * Mathf.Rad2Deg;
        //return Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
    }

    #region TO BE REMOVED

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

    // take it from between [0;max] to [-180;0]U[0;180] will not work with our inverse mercator projection
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

    #endregion
}
