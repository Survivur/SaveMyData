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
    }

    public override void OnConnectedToMaster()
    {
        print("서버접속 완료");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        SetButtonsActive(true);
    }

    /*public void OnDisconnected() => print("채팅 서버 연결 끊김");
    public void OnChatStateChange(ChatState state) { }
    public new void OnConnected() => chatClient.Subscribe(new string[] { "Lobby" });
    public void OnSubscribed(string[] channels, bool[] results) => print("채팅방 입장 완료");
    public void OnUnsubscribed(string[] channels) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }

    public void OnPrivateMessage(string sender, object message, string channeName)
    {
        print($"[개인 메시지]{sender}:{message}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            chatDisplay.text += $"{senders[i]}: {messages[i]}\n";
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        UnityEngine.Debug.Log($"[Photon Chat Debug] {level}: {message}");
    }*/

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
