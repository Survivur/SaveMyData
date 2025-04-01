using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        int playerNumber = PhotonNetwork.IsMasterClient ? 0 : 1;
        Vector3 spawnPosition = spawnPoints[playerNumber].position;

        PhotonNetwork.Instantiate(
            "Player",
            spawnPosition,
            Quaternion.identity);
    }
}
