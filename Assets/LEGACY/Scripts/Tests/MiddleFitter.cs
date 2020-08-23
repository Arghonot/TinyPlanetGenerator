using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MiddleFitter : MonoBehaviour
{
    public bool Run;

    public Transform A;
    public Transform B;
    public Transform Target;

    void Update()
    {
        if (Run)
        {
            Target.position = (A.position + B.position) / 2f;

            Run = false;
        }
    }
}
