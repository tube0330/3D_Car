using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    public WheelCollider targetWheel;
    public Vector3 wheelPosition = Vector3.zero;
    public Quaternion wheelRotation = Quaternion.identity;

    void Start()
    {

    }

    void Update()
    {
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}
