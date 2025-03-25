using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Diagnostics;


public class Network : MonoBehaviourPunCallbacks
{
    public TMP_Text Status;
    public TMP_InputField nicknameInput, roomInput;

    public Button disconnectButton;
    public Button RobbyButton;
    public Button RoomMakeButton;
    public Button RoomInButton;
    public Button RoomOutButton;
    public Button sendChatButton;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        DontDestroyOnLoad(gameObject);
    }
    /*void Update()
    {
        Status.text = PhotonNetwork.NetworkClientState.ToString();
    }*/

    private void Start()
    {
        SetButtonsActive(false);
        disconnectButton.onClick.AddListener(Disconnect);
        RobbyButton.onClick.AddListener(Robby);
        RoomMakeButton.onClick.AddListener(RoomMake);
        RoomInButton.onClick.AddListener(RoomIn);
        RoomOutButton.onClick.AddListener(RoomOut);
        PhotonNetwork.ConnectUsingSettings();
        nicknameInput.onEndEdit.AddListener(SetNickname);
    }

    public override void OnConnectedToMaster()
    {
        print("서버접속 완료");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        SetButtonsActive(true);
    }

    void SetNickname(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }


    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        print("서버연결 끊김");
        SceneManager.LoadScene("StartScene"); // StartScene으로 이동
    }

    /*public override void OnDisconnected(DisconnectCause cause)
    {
        print("서버연결 끊김");
    }*/

    public void Robby() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby()
    {
        print("로비 접속 완료");
        SceneManager.LoadScene("LobbyScene");
    }

    public void RoomMake() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    public override void OnCreatedRoom() => print("방 만들기 완료");
    public override void OnCreateRoomFailed(short returnCode, string message) => print("방 만들기 실패");

    public void RoomIn() => PhotonNetwork.JoinRoom(roomInput.text);
    public override void OnJoinedRoom() => print("방 참가 완료");
    public override void OnJoinRandomFailed(short returnCode, string message) => print("방 참가 실패");

    public void RoomOut() => PhotonNetwork.LeaveRoom();
    public override void OnLeftRoom() => print("방 떠남");

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);
            string playerStr = "방에 있는 플레이어 목록 : ";
            foreach (var player in PhotonNetwork.PlayerList) playerStr += player.NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }

    void SetButtonsActive(bool isActive)
    {
        disconnectButton.gameObject.SetActive(isActive);
        RobbyButton.gameObject.SetActive(isActive);
        RoomMakeButton.gameObject.SetActive(isActive);
        RoomInButton.gameObject.SetActive(isActive);
        RoomOutButton.gameObject.SetActive(isActive);
    }
}
