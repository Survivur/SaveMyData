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
        // 시작할 때는 플레이 버튼만 비활성화
        playButton.gameObject.SetActive(false);

        // 방에 입장한 상태인지 먼저 확인
        if (PhotonNetwork.InRoom)
        {
            UpdatePlayerList();
        }
        else
        {
            Debug.Log("아직 방에 입장하지 않았습니다. 방 입장 시 플레이어 목록이 업데이트됩니다.");
            playerListText.text = "방에 입장 중...";
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료! 플레이어 목록을 업데이트합니다.");
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

        sb.AppendLine("현재 입장한 플레이어");
        sb.AppendLine("-------------------");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            sb.AppendLine(player.NickName);
        }
        playerListText.text = sb.ToString();

        // 방에 2명이 되면 플레이 버튼 활성화
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
