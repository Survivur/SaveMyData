using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Text;
using TMPro;



public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playerListText;
    public Button playButton;

    void Start()
    {
        // ������ ���� �÷��� ��ư�� ��Ȱ��ȭ
        playButton.gameObject.SetActive(false);

        // �濡 ������ �������� ���� Ȯ��
        if (PhotonNetwork.InRoom)
        {
            UpdatePlayerList();
        }
        else
        {
            Debug.Log("���� �濡 �������� �ʾҽ��ϴ�. �� ���� �� �÷��̾� ����� ������Ʈ�˴ϴ�.");
            playerListText.text = "�濡 ���� ��...";
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� �Ϸ�! �÷��̾� ����� ������Ʈ�մϴ�.");
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        if (playerListText == null)
        {
            Debug.LogError("playerListText is not assigned.");
            return;
        }

        if (PhotonNetwork.PlayerList == null)
        {
            Debug.LogError("PhotonNetwork.PlayerList is null.");
            return;
        }

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("���� ������ �÷��̾�");
        sb.AppendLine("-------------------");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            sb.AppendLine(player.NickName);
        }
        playerListText.text = sb.ToString();

        // �濡 2���� �Ǹ� �÷��� ��ư Ȱ��ȭ
        playButton.gameObject.SetActive(PhotonNetwork.CurrentRoom.PlayerCount == 2);
    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }
}
