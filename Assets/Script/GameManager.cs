using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            if (spawnPoints.Length == 0)
            {
                Debug.LogWarning("spawnPoint is Empty!!");
            }

            int randomIndex = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[randomIndex].position;

            PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("룸에 참가하지 않은 상태입니다.");
        }
    }
}

