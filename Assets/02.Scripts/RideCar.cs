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

    public void PlayerGetInCar()
    {
        isRide = true;
        FPSPlayer.SetActive(false);
        mainCam.depth = 0f;
        /* AudioListener listener = mainCam.GetComponent<AudioListener>();
        listener.enabled = true; */
        FPSPlayer.transform.GetChild(0).GetComponent<Camera>().depth = -1;
        /* listener = FPSPlayer.GetComponent<AudioListener>();
        listener.enabled = false; */
    }

    private void PlayerGetOutCar()
    {
        FPSPlayer.transform.position = takeoffBox.transform.position;

        isRide = false;
        FPSPlayer.SetActive(true);
        mainCam.depth = 0;
        /* AudioListener listener = mainCam.GetComponent<AudioListener>();
        listener.enabled = false;
        listener = FPSPlayer.GetComponent<AudioListener>(); */
        FPSPlayer.transform.GetChild(0).GetComponent<Camera>().depth = -1; ;
    }
}
