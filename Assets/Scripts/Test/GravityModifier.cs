using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour
{
    public Transform GravitySource = null;

    void Update()
    {
        if (GravitySource != null)
        {
            Physics.gravity =  transform.position.normalized * -9.81f;
        }
    }
}
