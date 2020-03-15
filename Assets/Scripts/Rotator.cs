using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotation;

    void LateUpdate()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
