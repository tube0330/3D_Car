using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    private int PlayerClamp;
    private int Count;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        PlayerClamp = PhotonNetwork.CurrentRoom.MaxPlayers;
        Count = PhotonNetwork.CurrentRoom.PlayerCount;
        if (PlayerClamp > 4) return;
        CreateCar(Count);
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    private void CreateCar(int Count)
    {
        switch (Count)
        {
            case 1:
                Vector3 SpawnPos1 = new(-50, 2, 0);
                Quaternion SpawnRot1 = Quaternion.Euler(0f, 89f, 0f);
                PhotonNetwork.Instantiate(nameof(PlayerCar), SpawnPos1, SpawnRot1, 0, null);
                break;
            case 2:
                Vector3 SpawnPos2 = new(0, 2, 0);
                Quaternion SpawnRot2 = Quaternion.Euler(0f, 89f, 0f);
                PhotonNetwork.Instantiate(nameof(PlayerCar), SpawnPos2, SpawnRot2, 0, null);
                break;
            case 3:
                Vector3 SpawnPos3 = new(50, 2, 0);
                Quaternion SpawnRot3 = Quaternion.Euler(0f, 89f, 0f);
                PhotonNetwork.Instantiate(nameof(PlayerCar), SpawnPos3, SpawnRot3, 0, null);
                break;
            case 4:
                Vector3 SpawnPos4 = new(100, 2, 0);
                Quaternion SpawnRot4 = Quaternion.Euler(0f, 89f, 0f);
                PhotonNetwork.Instantiate(nameof(PlayerCar), SpawnPos4, SpawnRot4, 0, null);
                break;
        }
    }
}
