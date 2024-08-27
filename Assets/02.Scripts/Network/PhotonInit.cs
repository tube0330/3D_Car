using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Photon_InitialSet : MonoBehaviourPunCallbacks
{
    public InputField UserID;
    public Button JoinRandomButton;

    public string gameVersion = "1.0";

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            JoinRandomButton.interactable = false;
            PhotonNetwork.GameVersion = gameVersion;
            //PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            //Log.text = "Connect to Master Server...";
        }
    }

    public override void OnConnectedToMaster()
    {
        print("마스터서버 연결 완");
        UserID.text = GetUserID();
        PhotonNetwork.JoinLobby();
    }
    private string GetUserID()
    {
        string userID = PlayerPrefs.GetString("UserID");
        if (string.IsNullOrEmpty(userID))
            userID = "UserID_" + Random.Range(0, 999).ToString("000");
        return userID;
    }
    public override void OnJoinedLobby()
    {
        print("로비에 입장");
        JoinRandomButton.interactable = true;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("방 없음");
        //Log.text = "Error : Join Random Room Failed!";
        PhotonNetwork.CreateRoom("*Room", new RoomOptions { MaxPlayers = 3 });
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Log.text = "Error : Create Room Failed!";
        print("방 만들기 실패" + message);
    }
    public override void OnCreatedRoom()
    {
        //Log.text = "Create Room Success!";
    }
    public override void OnJoinedRoom()
    {
        print("방 만들기 성공");
        //Log.text = "Join Room Success!";
        StartCoroutine(LoadScene());
    }

    public void OnClickJoinRandomRoom()
    {
        print("방 입장 완");
        JoinRandomButton.interactable = false;
        PhotonNetwork.NickName = UserID.text;
        PlayerPrefs.SetString("UserID", UserID.text);
        PhotonNetwork.JoinRandomRoom();
    }

    private IEnumerator LoadScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation ao = SceneManager.LoadSceneAsync("F1TrackDisplayScene");
        yield return ao;
    }
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }    
}
