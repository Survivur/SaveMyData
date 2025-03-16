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
    public TMP_Text chatDisplay; // ä�� �޽��� ���
    public TMP_InputField chatInput; // ä�� �Է�â
    public Button sendChatButton; // ä�� ���� ��ư
    public Button leaveLobbyButton; // �κ� ������ ��ư

    private ChatClient chatClient;
    private string userId;

    void Start()
    {
        userId = PhotonNetwork.NickName; // �÷��̾� �г��� ����
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(userId));

        sendChatButton.onClick.AddListener(SendChatMessage);
        leaveLobbyButton.onClick.AddListener(LeaveLobby);
    }

    void Update()
    {
        if (chatClient != null)
            chatClient.Service(); // ä�� �޽��� ����
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
        SceneManager.LoadScene("StartScene"); // ���� ������ �̵�
    }

    // IChatClientListener �������̽� ����
    public void OnConnected() => chatClient.Subscribe(new string[] { "Lobby" });
    public void OnDisconnected() => Debug.Log("Chat Disconnected");
    public void OnChatStateChange(ChatState state) { }
    public void OnSubscribed(string[] channels, bool[] results) => Debug.Log("ä�ù� ����");
    public void OnUnsubscribed(string[] channels) => Debug.Log("ä�ù� ����");

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

