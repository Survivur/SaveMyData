using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class Network : MonoBehaviourPunCallbacks
{
    //public TMP_Text Status;
    public TMP_InputField nicknameInput, roomInput;

    public Button disconnectButton;
    public Button RobbyButton;
    public Button RoomMakeButton;
    public Button RoomInButton;
    
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

        // 버튼 리스너 등록
        disconnectButton.onClick.AddListener(Disconnect);
        RobbyButton.onClick.AddListener(Robby);
        RoomMakeButton.onClick.AddListener(RoomMake);
        RoomInButton.onClick.AddListener(RoomIn);
        nicknameInput.onEndEdit.AddListener(SetNickname);

        // 연결 상태 확인 후 연결 시도
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("서버에 연결을 시도합니다.");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("이미 서버에 연결되어 있습니다.");
            // 이미 연결된 상태라면 버튼들을 활성화
            SetButtonsActive(true);
        }


    }

    public override void OnConnectedToMaster()
    {
        print("서버접속 완료");

        // nicknameInput이 null이 아닌지 확인
        if (nicknameInput != null && !string.IsNullOrEmpty(nicknameInput.text))
        {
            PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        }
        else
        {
            // 기본 닉네임 설정
            PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(1000, 9999);
            Debug.LogWarning("닉네임 입력필드가 없어서 임시 닉네임을 설정했습니다: " + PhotonNetwork.LocalPlayer.NickName);
        }

        SetButtonsActive(true);
    }

    void SetNickname(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }


    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            print("서버연결 끊김");
        }
        SceneManager.LoadScene("StartScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"서버와 연결이 끊어졌습니다. 원인: {cause}");
        SetButtonsActive(false);

       
    }

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
    public override void OnJoinedRoom()
    {
        print("방 참가 완료");
        //SceneManager.LoadScene("RoomScene");
    }
    public override void OnJoinRandomFailed(short returnCode, string message) => print("방 참가 실패");

    //public void RoomOut() => PhotonNetwork.LeaveRoom();
    //public override void OnLeftRoom() => print("방 떠남");

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
        // 각 버튼이 null인지 확인하고 활성화/비활성화
        if (disconnectButton != null)
            disconnectButton.gameObject.SetActive(isActive);

        if (RobbyButton != null)
            RobbyButton.gameObject.SetActive(isActive);

        if (RoomMakeButton != null)
            RoomMakeButton.gameObject.SetActive(isActive);

        if (RoomInButton != null)
            RoomInButton.gameObject.SetActive(isActive);
    }
}
