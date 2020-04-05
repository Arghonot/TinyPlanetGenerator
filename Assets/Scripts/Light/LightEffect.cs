using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : Singleton<LightEffect>
{
    public float SearchRange;
    public Transform player;

    public float LerpDuration;

    Transform[] planets;

    Vector3 velocity = Vector3.zero;

    // TODO change this into a coroutine
    void Update()
    {
        // TODO Ugly as F -> remove this asap
        planets = SolarSystemManager.Instance.planets.Select(x => x.transform).ToArray();

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(
                GetPlanetClusterCenter(getClosestPlanet()),
                Vector3.up),
            Time.deltaTime * LerpDuration);

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
}
