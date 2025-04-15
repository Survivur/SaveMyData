using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Diagnostics;


public class LobbyManager : MonoBehaviour, IChatClientListener
{
    public TMP_Text chatDisplay; // 채팅 메시지 출력
    public ScrollRect chatScrollRect;
    public TMP_InputField chatInput; // 채팅 입력창
    public Button sendChatButton; // 채팅 전송 버튼
    public Button leaveLobbyButton; // 로비 나가기 버튼

    private ChatClient chatClient;
    private string userId;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (chatClient == null || !chatClient.CanChat)   
            {
                string userId = PhotonNetwork.NickName;
                string appIdChat = "b60ab86d-83ae-449d-8719-543534979597";
                chatClient = new ChatClient(this);
                chatClient.UseBackgroundWorkerForSending = true;
                chatClient.Connect(appIdChat, "1.0", new Photon.Chat.AuthenticationValues(userId));

                DebugReturn(DebugLevel.INFO,"[LobbyManager] 채팅 서버에 다시 연결 시도");
                
            }

            DebugReturn(DebugLevel.INFO, "[Start] chatClient.Connect() 호출 완료");

        }
        else
        {
            DebugReturn(DebugLevel.WARNING, "[LobbyManager] Photon 서버 연결이 끊어졌습니다. 다시 연결을 시도합니다.");
            PhotonNetwork.ConnectUsingSettings(); // Photon 서버 재연결
        }
        sendChatButton.onClick.AddListener(SendChatMessage);
        leaveLobbyButton.onClick.AddListener(LeaveLobby);
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service(); // 채팅 메시지 수신
        }
    }

    public void SendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            chatClient.PublishMessage("Lobby", chatInput.text);
            chatInput.text = "";
        }
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
        //PhotonNetwork.Disconnect();
        SceneManager.LoadScene("SecondScene"); // 두번째 씬으로 이동
    }

    // IChatClientListener 인터페이스 구현
    public void OnConnected()
    {
        if(chatClient == null)
        {
            DebugReturn(DebugLevel.ERROR, "[OnConnected] chatClient가 null입니다. 채팅 연결이 정상적으로 이루어지지 않았습니다.");
            return;
        }

        DebugReturn(DebugLevel.INFO, "[OnConnected] 채팅 서버 연결 성공!");
        chatClient.Subscribe(new string[] { "Lobby" });
    }

    public void OnDisconnected()
    {
        DebugReturn(DebugLevel.WARNING, "[OnDisconnected] 채팅 서버 연결이 끊어졌습니다!!!");

        if(chatClient != null)
        {
            string userId = PhotonNetwork.NickName;
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(userId));
            DebugReturn(DebugLevel.INFO, "[OnDisconnected] 채팅 서버 재연결 시도 중...");
            
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        DebugReturn(DebugLevel.INFO, $"[OnChatStateChange] 상태 변경: {state}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        DebugReturn(DebugLevel.INFO, $"[OnSubscribed] 채팅방 입장");
    }

    public void OnUnsubscribed(string[] channels)
    {
        DebugReturn(DebugLevel.INFO, $"[OnUnsubscribed] 채팅방 퇴장");
    }

    public Transform chatContent;
    public GameObject chatMessagePrefab;

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            GameObject newMessage = Instantiate(chatMessagePrefab, chatContent);
            TMP_Text messageText = newMessage.GetComponent<TMP_Text>();

            messageText.fontSize = 14;
            messageText.enableAutoSizing = true;
            messageText.text = $"{messages[i].ToString()}";

            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0f;

        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void DebugReturn(DebugLevel level, string message)
    {
        UnityEngine.Debug.Log($"[Chat {level}] {message}");
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}

