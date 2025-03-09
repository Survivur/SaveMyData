using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class Network : MonoBehaviourPunCallbacks
{
    public TMP_Text Status;
    public TMP_InputField nicknameInput, roomInput;

    public Button connectButton;
    public Button disconnectButton;
    public Button RobbyButton;
    public Button RoomMakeButton;
    public Button RoomInButton;
    public Button RoomOutButton;

    void Awake() => Screen.SetResolution(960, 540, false);
    void Update() => Status.text = PhotonNetwork.NetworkClientState.ToString();





    private void Start()
    {
        // �� ��ư�� OnClick �̺�Ʈ�� �Լ� ����
        connectButton.onClick.AddListener(Connect);
        disconnectButton.onClick.AddListener(Disconnect);
        RobbyButton.onClick.AddListener(Robby);
        RoomMakeButton.onClick.AddListener(RoomMake);
        RoomInButton.onClick.AddListener(RoomIn);
        RoomOutButton.onClick.AddListener(RoomOut);
    }


    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        print("�������� �Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("�������� ����");
    }

    public void Robby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        print("�κ� ���� �Ϸ�");
    }

    public void RoomMake() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    public override void OnCreatedRoom()
    {
        print("�� ����� �Ϸ�");
    }
    public void RoomIn() => PhotonNetwork.JoinRoom(roomInput.text);
    public override void OnJoinedRoom()
    {
        print("�� ���� �Ϸ�");
    }
    public void RoomOut() => PhotonNetwork.LeaveRoom();
    public override void OnLeftRoom()
    {
        print("�� ����");
    }
}
