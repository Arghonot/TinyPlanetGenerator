using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesBehaviour : MonoBehaviour
{
    public Transform Player;
    public float MaxDistance;

    void Update()
    {
        // So we hide the rules
        if (Vector3.Distance(Player.position, transform.position) > MaxDistance)
        {
            Destroy(gameObject);
        }
    }
}
