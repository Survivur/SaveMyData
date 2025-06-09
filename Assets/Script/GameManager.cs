using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;
using TMPro;



public class GameManager : MonoBehaviour
{
    public int stage;
    public float playTime;
    public bool isBattle;
    public TMP_Text stageTxt;
    public TMP_Text playTimeTxt;
    public TMP_Text playerHealthTxt;
    public TMP_Text playerAmmoTxt;
    public TMP_Text playerNameTxt;
    public TMP_Text playerWeaponTxt;
    public string[] Weapon = new string[] { "Minigun", "Shotgun", "Rocket", "Knife", "Smg" };
    [SerializeField, ReadOnly(true)] private Transform[] spawnPoints;

    [SerializeField, ReadOnly] private static GameObject player;
    public static GameObject Player => player;
    [SerializeField, ReadOnly] private static GameObject[] enemy;
    public static GameObject[] Enemy;
    private bool hasSpawned = false;
private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Weapon weapon = GetComponent<Weapon>();
            if (weapon != null)
            {
                // 획득한 무기 이름을 UI에 표시
                playerWeaponTxt.text = weapon.weaponName;
            }

            // 무기를 획득했으므로 사라지도록 설정
            Destroy(gameObject);
        }
    }
    void Start()
    {
        TrySpawn();
    }


    private void TrySpawn()
    {
        if (hasSpawned) return; // �ߺ� ����
        if (!PhotonNetwork.InRoom) return;

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("spawnPoints�� ��� �ֽ��ϴ�.");
            return;
        }

        //int randomIndex = Random.Range(0, spawnPoints.Length);
        //Vector3 spawnPos = spawnPoints[randomIndex].position;

        //player = PhotonNetwork.Instantiate("Prefabs/Player", spawnPos, Quaternion.identity);
        hasSpawned = true;

        Debug.Log("�÷��̾� ���� �Ϸ�: " + player.name);
    }
    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }
    void LateUpdate()
    {
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        stageTxt.text = "STAGE" + stage;
        
    }
  

}