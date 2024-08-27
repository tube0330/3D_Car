using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Cinemachine;
using UnityEngine;

public class CameraSetup : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine)
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); 
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
        
    }
}
