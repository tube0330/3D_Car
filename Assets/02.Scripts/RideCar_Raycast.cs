using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideCar_Raycast : MonoBehaviour
{
    public int carLayer = LayerMask.NameToLayer("Car");
    Camera cam;
    RideCar C_ridecar;

    void Start()
    {
        cam = this.gameObject.GetComponentInChildren<Camera>();
        C_ridecar = GetComponent<RideCar>();
    }

    void Update()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, 25f, carLayer))
        {
            if (Input.GetKeyDown(KeyCode.E))
                C_ridecar.PlayerGetInCar();
        }
    }
}