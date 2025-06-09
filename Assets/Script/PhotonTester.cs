using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonTest : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // 마스터 서버 연결 시도
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결 완료");
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");

        // 확인용 프리팹 생성
        //PhotonNetwork.Instantiate("Prefabs/Player", Vector3.zero, Quaternion.identity);
    }
}