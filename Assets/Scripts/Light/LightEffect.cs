using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : Singleton<LightEffect>
{
    public float SearchRange;
    public Transform player;

    public float LerpDuration;

    Transform[] planets;

    public Vector3 currentPointTolook = Vector3.zero;
    public Vector3 NextPointToLook;

    Vector3 velocity = Vector3.zero;

    // TODO change this into a coroutine
    void Update()
    {
        // TODO Ugly as F -> remove this asap
        planets = SolarSystemManager.Instance.planets.Select(x => x.transform).ToArray();

        NextPointToLook = GetPlanetClusterCenter(getClosestPlanet());

        transform.LookAt(Vector3.SmoothDamp(player.transform.position, NextPointToLook, ref velocity, LerpDuration));

        transform.LookAt(currentPointTolook);
        //Vector3 newpoint = Vector3.Lerp(currentPointTolook, NextPointToLook, Time.deltaTime * LerpDuration);

        //transform.LookAt(newpoint);
        //print(newpoint);

        //currentPointTolook = newpoint;
    }

    /// <summary>
    /// Return the barycenter of all the objects tagged as POI around the planet[ClosestPlanetIndex].
    /// </summary>
    /// <param name="ClosestPlanetIndex">The index of the closest planet to the player.</param>
    /// <returns>The barycenter of all the object in SearchRange.</returns>
    Vector3 GetPlanetClusterCenter(int ClosestPlanetIndex)
    {
        Collider[] POIs = Physics.OverlapSphere(
            planets[ClosestPlanetIndex].position,
            SearchRange,
            LayerMask.NameToLayer("POI"));
        Vector3 center = Vector3.zero;

        if (POIs.Length == 1)
        {
            return POIs[0].transform.position;
        }

        for (int i = 0; i < POIs.Length; i++)
        {
            center += POIs[i].transform.position;
        }

        center /= POIs.Length;

        return center;
    }

    int getClosestPlanet()
    {
        int index = 0;
        float distance = float.MaxValue;
        float currentDistance = 0f;

        for (int i = 0; i < planets.Length; i++)
        {
            currentDistance = Vector3.Distance(planets[i].position, player.position);

            if (currentDistance < distance)
            {
                index = i;
                distance = currentDistance;
            }
        }

        return index;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(NextPointToLook, 20f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(currentPointTolook, 20f);
    }
}
