using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that move the camera, based on Brackeys's tutorial "MULTIPLE TARGET CAMERA in Unity" :
/// "https://www.youtube.com/watch?v=aLpixrPvlB8"
/// </summary>
public class MultipleTargetsCamera : MonoBehaviour
{
    public List<Transform> Targets;
    public Vector3 offset;
    public float smoothTime;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float ZoomLimiter = 50f;

    Vector3 OriginalOffset;
    Vector3 OriginalRotation;

    Camera cam;
    Vector3 velocity;

    #region UNITY API

    private void Start()
    {
        cam = GetComponent<Camera>();
        OriginalOffset = offset;
        OriginalRotation = transform.eulerAngles;
    }

    void LateUpdate()
    {
        if (Targets == null || Targets.Count == 0)
        {
            return;
        }

        Move();
        Zoom();

    }

    #endregion

    #region PUBLIC FUNCTIONS

    public void RevertToOriginRotation()
    {
        transform.eulerAngles = OriginalRotation;
    }

    public void RevertToOriginOffset()
    {
        offset = OriginalOffset;
    }

    #endregion

    #region RUNTIME MOVEMENTS

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / ZoomLimiter);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    #endregion

    #region Target management

    /// <summary>
    /// Add a new target to focus the camera to if it wasn't already on the camera's targets.
    /// </summary>
    /// <param name="newtarget">The new target.</param>
    public void AddTarget(Transform newtarget)
    {
        if (!Targets.Contains(newtarget))
        {
            Targets.Add(newtarget);
        }
    }

    /// <summary>
    /// Remove a camera's target if.
    /// </summary>
    /// <param name="target">The target to be removed</param>
    public void RemoveTarget(Transform target)
    {
        if (Targets.Contains(target))
        {
            Targets.Remove(target);
        }
    }

    #endregion

    #region MISCS

    float GetGreatestDistance()
    {
        var bounds = new Bounds(Targets[0].position, Vector3.zero);

        for (int i = 0; i < Targets.Count - 1; i++)
        {
            for (int x = 0; x < Targets[i].childCount; x++)
            {
                bounds.Encapsulate(Targets[i].position);
            }
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        if (Targets.Count == 1)
        {
            return Targets[0].position;
        }

        var bounds = new Bounds(Targets[0].position, Vector3.zero);

        for (int i = 0; i < Targets.Count; i++)
        {
            bounds.Encapsulate(Targets[i].position);
        }

        return bounds.center;
    }

    #endregion
}
