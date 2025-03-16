using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviour, IChatClientListener
{
    public TMP_Text chatDisplay; // 채팅 메시지 출력
    public TMP_InputField chatInput; // 채팅 입력창
    public Button sendChatButton; // 채팅 전송 버튼
    public Button leaveLobbyButton; // 로비 나가기 버튼

    private ChatClient chatClient;
    private string userId;

    void Start()
    {
        userId = PhotonNetwork.NickName; // 플레이어 닉네임 설정
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(userId));

        sendChatButton.onClick.AddListener(SendChatMessage);
        leaveLobbyButton.onClick.AddListener(LeaveLobby);
    }

    void Update()
    {
        if (chatClient != null)
            chatClient.Service(); // 채팅 메시지 수신
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
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("StartScene"); // 시작 씬으로 이동
    }

    // IChatClientListener 인터페이스 구현
    public void OnConnected() => chatClient.Subscribe(new string[] { "Lobby" });
    public void OnDisconnected() => Debug.Log("Chat Disconnected");
    public void OnChatStateChange(ChatState state) { }
    public void OnSubscribed(string[] channels, bool[] results) => Debug.Log("채팅방 입장");
    public void OnUnsubscribed(string[] channels) => Debug.Log("채팅방 퇴장");

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            chatDisplay.text += $"{senders[i]}: {messages[i]}\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void DebugReturn(DebugLevel level, string message) => Debug.Log($"[Chat Debug] {message}");
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}

