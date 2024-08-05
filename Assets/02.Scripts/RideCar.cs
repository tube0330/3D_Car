using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideCar : MonoBehaviour
{
    string playerTag = "Player";
    public bool isRide = false;
    public GameObject FPSPlayer;
    public Camera mainCam;
    public GameObject takeoffBox;

    void Start()
    {
        FPSPlayer = GameObject.FindWithTag(playerTag);
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            PlayerGetOutCar();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            PlayerGetInCar();
        }
    }

    private void PlayerGetInCar()
    {
        isRide = true;
        FPSPlayer.SetActive(false);
        mainCam.depth = 0f;
        //AudioListener listener = mainCam
        FPSPlayer.transform.GetChild(0).GetComponent<Camera>().depth = -1;
    }

    private void PlayerGetOutCar()
    {
        FPSPlayer.transform.position = takeoffBox.transform.position;

        isRide = false;
        FPSPlayer.SetActive(true);
        mainCam.depth = 0;
        FPSPlayer.transform.GetChild(0).GetComponent<Camera>().depth = -1; ;
    }
}
