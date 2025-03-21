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
        print("�������� �Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        SetButtonsActive(true);
    }

    /*public void OnDisconnected() => print("ä�� ���� ���� ����");
    public void OnChatStateChange(ChatState state) { }
    public new void OnConnected() => chatClient.Subscribe(new string[] { "Lobby" });
    public void OnSubscribed(string[] channels, bool[] results) => print("ä�ù� ���� �Ϸ�");
    public void OnUnsubscribed(string[] channels) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }

    public void OnPrivateMessage(string sender, object message, string channeName)
    {
        print($"[���� �޽���]{sender}:{message}");
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
        print("�������� ����");
        SceneManager.LoadScene("StartScene"); // StartScene���� �̵�
    }

    /*public override void OnDisconnected(DisconnectCause cause)
    {
        print("�������� ����");
    }*/

    public void Robby() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby()
    {
        print("�κ� ���� �Ϸ�");
        SceneManager.LoadScene("LobbyScene");
    }

    public void RoomMake() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    public override void OnCreatedRoom() => print("�� ����� �Ϸ�");
    public override void OnCreateRoomFailed(short returnCode, string message) => print("�� ����� ����");

    public void RoomIn() => PhotonNetwork.JoinRoom(roomInput.text);
    public override void OnJoinedRoom() => print("�� ���� �Ϸ�");
    public override void OnJoinRandomFailed(short returnCode, string message) => print("�� ���� ����");

    public void RoomOut() => PhotonNetwork.LeaveRoom();
    public override void OnLeftRoom() => print("�� ����");

    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);
            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            foreach (var player in PhotonNetwork.PlayerList) playerStr += player.NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
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
