using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float rotSpeed;

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    Vector3 velocity;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;


    void Update()
    {
        HandleInputs();
    }

    void HandleInputs()
    {
        Vector2 playerInput;

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector3 velocity =
            new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        Vector3 acceleration =
            new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        Vector3 desiredVelocity =
            new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;

        if (playerInput.magnitude > Vector3.kEpsilon)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Inverse(Quaternion.LookRotation(
                    new Vector3(displacement.x, 0f, -displacement.z),
                    Vector3.up)),
                Time.deltaTime * rotSpeed);
        }

        transform.localPosition += displacement;
    }    
}
