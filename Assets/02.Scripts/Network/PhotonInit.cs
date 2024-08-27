using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public InputField inputField_ID;
    public Button Button_join;
    string userID;
    string gameVersion = "V1.0";

    void Awake()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        print("마스터서버 연결 완");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("로비에 입장");
        inputField_ID.text = GetUserID();
        PhotonNetwork.JoinRandomRoom(); //로비 입장했으면 아무 방이나 접속하도록
    }

    void OnGUI()    //왼쪽 상단에 나타냄
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());   //photonNetwork에 클라이언트 상태 정보 알려줌
    }

    string GetUserID()
    {
        userID = PlayerPrefs.GetString("USER_ID");

        if (string.IsNullOrEmpty(userID))
            userID = "USER_" + Random.Range(0, 999).ToString();

        return userID;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("방 없음");
        PhotonNetwork.CreateRoom("*Room", new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnCreatedRoom()
    {
        print("방 만들기 성공");
    }

    public override void OnJoinedRoom()
    {
        print("방 입장");
        PhotonNetwork.NickName = inputField_ID.text;
    }

    public void OnButtonClick() => SceneManager.LoadScene("F1TrackDisplayScene");
}
