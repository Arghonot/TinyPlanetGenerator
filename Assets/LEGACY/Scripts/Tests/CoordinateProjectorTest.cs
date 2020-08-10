using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CoordinateProjectorTest : MonoBehaviour
{
    public bool Run1;
    public bool Run2;
    public bool Flush;
    public bool Print;

    public Transform mark;
    public float Rad;
    public Vector2 LnLat;

    void Update()
    {
        if (Run1)
        {
            Run1 = false;
            if (Print)
            {
                Debug.Log("[originalpos][Lon][Lat][Rad] : " +
                    mark.transform.position + " " +
                    CoordinatesProjector.CartesianToLon(mark.transform.position) + " " + 
                    CoordinatesProjector.CartesianToLat(mark.transform.position) + " " + 
                    CoordinatesProjector.CartesianToRadius(mark.transform.position));
            }
        }
        if (Run2)
        {
            Run2 = false;

            Vector3 pos = 
                CoordinatesProjector.InverseMercatorProjector(
                    LnLat.x * Mathf.Deg2Rad,
                    LnLat.y * Mathf.Deg2Rad,
                    Rad);
            mark.transform.position = pos;

            if (Print)
            {
                Debug.Log("[pos] : " + pos);
            }
        }
        if (Flush)
        {
            Debug.Log("+---------------------+");
        }
    }
}
