using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverBoard : MonoBehaviour
{
    public Vector3 RaycastDirection = Vector3.up * -1f;
    public List<Transform> Reactors;

    public Transform Prop;
    public Transform CM;

    public float ForceMultiplier = 400f;
    public float TorqueMultiplier = 300f;
    public float BikeHeight = 3f;

    public float SpringsMultiplier = 250f;

    Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = CM.localPosition;
    }

    private void Update()
    {
        body.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical") * ForceMultiplier, Prop.transform.position);
        body.AddTorque(Time.deltaTime * transform.TransformDirection(Vector3.up) * Input.GetAxis("Horizontal") * TorqueMultiplier);

        foreach (var reactor in Reactors)
        {
            RaycastHit hit;

            if (Physics.Raycast(reactor.position, transform.TransformDirection(Vector3.down), out hit, BikeHeight))
            {
                body.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.up) * Mathf.Pow(BikeHeight - hit.distance, 2) / 3f * SpringsMultiplier, reactor.position);
            }

            Debug.Log(hit.distance);
        }

        //body.AddForce(-Time.deltaTime * transform.TransformVector(Vector3.right) * transform.InverseTransformVector(body.velocity).x * 5f);
    }

    //private void Start()
    //{
    //    body = GetComponent<Rigidbody>();
    //}

    //private void Update()
    //{
    //    UserInputs();
    //}

    //void UserInputs()
    //{
    //    Vector2 playerInput;

    //    playerInput.x = Input.GetAxis("Horizontal");
    //    playerInput.y = Input.GetAxis("Vertical");
    //    playerInput = Vector2.ClampMagnitude(playerInput, 1f);

    //    Vector3 velocity =
    //        new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

    //    Vector3 acceleration =
    //        new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

    //    Vector3 desiredVelocity =
    //        new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

    //    float maxSpeedChange = maxAcceleration * Time.deltaTime;

    //    velocity.x =
    //        Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
    //    velocity.z =
    //        Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

    //    Vector3 displacement = velocity * Time.deltaTime;

    //    body.AddForce(transform.TransformVector(acceleration) * forwardForce);

    //    body.AddTorque((Vector3.up * displacement.y) * TorqueForce);
    //}

    //private void FixedUpdate()
    //{
    //    Hover();
    //}

    //void Hover()
    //{
    //    for (int i = 0; i < Reactors.Count; i++)
    //    {
    //        HandleReactor(i);
    //    }
    //}

    //void HandleReactor(int index)
    //{
    //    RaycastHit hit = new RaycastHit();

    //    if (Physics.Raycast(new Ray(Reactors[index].transform.position, RaycastDirection), out hit))
    //    {
    //        float distance = Vector3.Distance(Reactors[index].position, hit.point);
    //        body.AddForceAtPosition((RaycastDirection * -HoverForce) / distance, Reactors[index].position);
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < Reactors.Count; i++)
    //    {
    //        Gizmos.DrawRay(Reactors[i].position, RaycastDirection);
    //    }
    //}
}
