using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameObject player;
    public static GameObject Player => player;
    private static GameObject[] enemy;
    public static GameObject[] Enemy;
    [SerializeField, ReadOnly(true)] private Transform[] spawnPoints;

    public int stage;
    public float playTime;
    public bool isBattle;
    public TMP_Text stageTxt;
    public TMP_Text playTimeTxt;
    public TMP_Text playerHealthTxt;
    public TMP_Text playerAmmoTxt;

    private bool hasSpawned = false;

    void Start()
    {
        TrySpawn();
    }

    void Update()
    {
        TrySpawn();

        if (isBattle)
            playTime += Time.deltaTime;
    }
    void LateUpdate()
    {
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = $"{hour:00}:{min:00}:{second:00}";
        // playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        stageTxt.text = "STAGE" + stage;
    }
    
    private void TrySpawn()
    {
        if (hasSpawned) return; // �ߺ� ����
        if (!PhotonNetwork.InRoom) return;

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("spawnPoints�� ���? �ֽ��ϴ�.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[randomIndex].position;

        player = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
        hasSpawned = true;

        Debug.Log("�÷��̾� ���� �Ϸ�: " + player.name);
    }

}