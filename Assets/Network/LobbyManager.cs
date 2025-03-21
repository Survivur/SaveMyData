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
    //public TMP_Text chatDisplay; // ä�� �޽��� ���
    public ScrollRect chatScrollRect;
    public TMP_InputField chatInput; // ä�� �Է�â
    public Button sendChatButton; // ä�� ���� ��ư
    public Button leaveLobbyButton; // �κ� ������ ��ư

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

                DebugReturn(DebugLevel.INFO,"[LobbyManager] ä�� ������ �ٽ� ���� �õ�");
                
            }

            DebugReturn(DebugLevel.INFO, "[Start] chatClient.Connect() ȣ�� �Ϸ�");

        }
        else
        {
            DebugReturn(DebugLevel.WARNING, "[LobbyManager] Photon ���� ������ ���������ϴ�. �ٽ� ������ �õ��մϴ�.");
            PhotonNetwork.ConnectUsingSettings(); // Photon ���� �翬��
        }
        sendChatButton.onClick.AddListener(SendChatMessage);
        leaveLobbyButton.onClick.AddListener(LeaveLobby);
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service(); // ä�� �޽��� ����
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
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("StartScene"); // ���� ������ �̵�
    }

    // IChatClientListener �������̽� ����
    public void OnConnected()
    {
        if(chatClient == null)
        {
            DebugReturn(DebugLevel.ERROR, "[OnConnected] chatClient�� null�Դϴ�. ä�� ������ ���������� �̷������ �ʾҽ��ϴ�.");
            return;
        }

        DebugReturn(DebugLevel.INFO, "[OnConnected] ä�� ���� ���� ����!");
        chatClient.Subscribe(new string[] { "Lobby" });
    }

    public void OnDisconnected()
    {
        DebugReturn(DebugLevel.WARNING, "[OnDisconnected] ä�� ���� ������ ���������ϴ�!!!");

        if(chatClient != null)
        {
            string userId = PhotonNetwork.NickName;
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(userId));
            DebugReturn(DebugLevel.INFO, "[OnDisconnected] ä�� ���� �翬�� �õ� ��...");
            
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        DebugReturn(DebugLevel.INFO, $"[OnChatStateChange] ���� ����: {state}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        DebugReturn(DebugLevel.INFO, $"[OnSubscribed] ä�ù� ����");
    }

    public void OnUnsubscribed(string[] channels)
    {
        DebugReturn(DebugLevel.INFO, $"[OnUnsubscribed] ä�ù� ����");
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

