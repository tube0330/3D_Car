using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideCar_Raycast : MonoBehaviour
{
    public int carLayer;
    Camera cam;
    RideCar ridecar;

    void Start()
    {
        cam = this.gameObject.GetComponentInChildren<Camera>();
        ridecar = GetComponent<RideCar>();
        carLayer = LayerMask.NameToLayer("Car");
    }

    void Update()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, 25f, carLayer))
        {
            Debug.Log("car");
            Debug.DrawRay(cam.transform.position, cam.transform.forward * 25f, Color.green);
            if (Input.GetKeyDown(KeyCode.E))
                ridecar.PlayerGetInCar();
        }
    }
}