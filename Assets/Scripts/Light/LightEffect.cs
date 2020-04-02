using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : Singleton<LightEffect>
{
    public Transform[] planets;
    public Transform player;

    public void Init()
    {
        planets = SolarSystemManager.Instance.planets.Select(x => x.transform).ToArray();
    }

    void Update()
    {
        transform.LookAt(planets[getClosestPlanet()]);
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
