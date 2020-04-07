using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesBehaviour : MonoBehaviour
{
    public Movement Player;
    public float SpawnOffset;
    public float MaxDistance;

    //private void Start()
    //{
    //    print(SolarSystemManager.Instance.AmountOfPlanets());

    //    Vector3 LastPlanet = SolarSystemManager.Instance.GetPlanetPosition(
    //        SolarSystemManager.Instance.AmountOfPlanets() - 1);
    //    Vector3 anchor = Vector3.right * SpawnOffset;

    //    print(LastPlanet);
    //    print(anchor);

    //    transform.position = (LastPlanet + anchor);
    //    Player.Reposition(
    //        transform.position + 
    //        (Vector3.right * SpawnOffset));
    //}

    void Update()
    {
        // So we hide the rules
        if (Vector3.Distance(Player.transform.position, transform.position) > MaxDistance)
        {
            Destroy(gameObject);
        }
    }
}
