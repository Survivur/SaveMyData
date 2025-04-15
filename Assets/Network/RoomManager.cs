using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;



public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playerListText;
    public Button playButton;
    public Button RoomOutButton;

    void Start()
    {
        // 시작할 때는 플레이 버튼만 비활성화
        playButton.gameObject.SetActive(false);
        RoomOutButton.gameObject.SetActive(true);

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
        playButton.gameObject.SetActive(PhotonNetwork.CurrentRoom.PlayerCount >= 1);
    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    public void OnClickRoomOut()
    {
        // 방을 나가기 전에 씬 전환을 예약
        StartCoroutine(LeaveRoomAndLoadScene());
    }

    private IEnumerator LeaveRoomAndLoadScene()
    {
        // 방을 떠나기
        PhotonNetwork.LeaveRoom();

        // 방을 완전히 나갈 때까지 대기
        while (PhotonNetwork.InRoom)
            yield return null;

        // 씬 전환 (PhotonNetwork.LoadLevel 대신 사용)
        SceneManager.LoadScene("SecondScene");
    }

    // 방을 나갔을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        Debug.Log("방에서 나왔습니다.");
    }
}
