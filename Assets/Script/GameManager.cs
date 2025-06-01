using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField, ReadOnly(true)] private Transform[] spawnPoints;
    
    [SerializeField, ReadOnly] private static GameObject player;
    public static GameObject Player => player;
    [SerializeField, ReadOnly] private static GameObject[] enemy;
    public static GameObject[] Enemy;

    private bool hasSpawned = false;

    void Start()
    {
        TrySpawn();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        TrySpawn();
    }


    private void TrySpawn()
    {
        if (hasSpawned) return; // 중복 방지
        if (!PhotonNetwork.InRoom) return;
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("spawnPoints가 비어 있습니다.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[randomIndex].position;

        player = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
        hasSpawned = true;

        Debug.Log("플레이어 생성 완료: " + player.name);
    }
}