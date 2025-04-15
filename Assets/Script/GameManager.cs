using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private Transform[] spawnPoints;

    void Start()
    {
        GameObject spawnParent = GameObject.Find("spawnPoints");
        spawnPoints = new Transform[spawnParent.transform.childCount];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = spawnParent.transform.GetChild(i);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            int index = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[index];

            PhotonNetwork.Instantiate("Prefabs/Player", spawnPoint.position, spawnPoint.rotation);
        }
    }
}

