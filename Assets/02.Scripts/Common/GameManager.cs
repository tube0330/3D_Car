using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    static GameManager instance;
    public static GameManager g_instance
    {
        get { return instance; }
        set
        {
            if (instance == null)
                instance = value;
            else if (instance != value)
                Destroy(value.gameObject);
        }
    }

    [SerializeField] GameObject carPrefabs;
    /* [SerializeField] List<Transform> spawnList;
    [SerializeField] int index; */
    int playerClamp;
    int cnt;

    void Awake()
    {
        playerClamp = PhotonNetwork.CurrentRoom.MaxPlayers;
        cnt = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerClamp > 3) return;

        CreateCar(cnt);
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    /* void Start()
    {
        index = 0;
        var spawnPoint = GameObject.Find("SpawnPoint").gameObject;
        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(spawnList);
        spawnList.RemoveAt(0);
    } */

    void CreateCar(int cnt)
    {
        switch (cnt)
        {
            case 1:
                Vector3 point1 = new Vector3(0f, 0f, -10f);
                Quaternion rot1 = Quaternion.Euler(0f, 90f, 0f);
                PhotonNetwork.Instantiate(carPrefabs.name, point1, rot1);
                break;

            case 2:
                Vector3 point2 = new Vector3(0f, 0f, 0f);
                Quaternion rot2 = Quaternion.Euler(0f, 90f, 0f);
                PhotonNetwork.Instantiate(carPrefabs.name, point2, rot2);
                break;

            case 3:
                Vector3 point3 = new Vector3(0f, 0f, 10f);
                Quaternion rot3 = Quaternion.Euler(0f, 90f, 0f);
                PhotonNetwork.Instantiate(carPrefabs.name, point3, rot3);
                break;
        }
    }
}
