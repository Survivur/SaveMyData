using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerGun : MonoBehaviour, IShootable
{
    [Header("Options")]
    [SerializeField] public Counter bulletCount = new Counter(10);
    [SerializeField] public float reloadSpeed = 1.5f;
    [SerializeField] protected float _bulletSpeed = 50f;
    [SerializeField] public float BulletSpeed => _bulletSpeed;

    [Header("Information")]
    [SerializeField, ReadOnly(true)] public float bulletDelay = 0.3f;
    [SerializeField, ReadOnly(true)] public float shootGap = 1f;

    [Header("Components", order = 0)]
    [SerializeField, ReadOnly(true)] private Player player = null;

    [SerializeField, ReadOnly] protected PhotonView photonView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CodeExtensions.SetIfUnityNull(ref player, gameObject.GetComponentCached<Player>());
        CodeExtensions.SetIfUnityNull(ref photonView, gameObject.GetComponentCached<PhotonView>());

        if (photonView.IsMine)
        {
            GameObjectRegistry.GetOrRegister("Canvas/Game Panel/GameObject/Player1/Status Group/Bullet Text").GetComponent<GetTextGUI>().getTextFunc = () => $"Bullet : {bulletCount.Count} / {bulletCount.Max}";
        }
    }

    public void Shoot(float damage, Vector2 dir, bool isBlockedByBlock = false)
    {
        // bullet count ?��?
        if (bulletCount != 0)
        {
            GameObject bulletObj = PhotonNetwork.Instantiate(
                ResourcePath.Bullet,
                transform.position + (Vector3)dir,
                Quaternion.identity);
            int bulletID = bulletObj.gameObject.GetComponentCached<PhotonView>().ViewID;

            photonView.RPC(nameof(SetBullet_RPC), RpcTarget.AllBuffered, bulletID,
                _bulletSpeed,
                damage,
                dir,
                isBlockedByBlock);

            bulletCount.Count--;

            if (bulletCount == 0)
                Reload();
        }
    }

    public void Shoot(Bullet bullet)
    {
        //��Ʈ��ũ�� ���� �Ѿ� ����
        GameObject bulletObj = PhotonNetwork.Instantiate(
            ResourcePath.Bullet,
            transform.position + (Vector3)bullet.dir,
            Quaternion.identity);
        int bulletID = bulletObj.GetComponent<PhotonView>().ViewID;

        photonView.RPC(nameof(SetBullet_RPC), RpcTarget.AllBuffered, bulletID,
         bullet.Speed,
         bullet.Damage,
         bullet.dir,
         bullet.TargetTags.Exists(val => val == Tags.Ground));
    }

    public void Reload()
    {
        bulletCount.Count = 0;
        Invoke(nameof(Reset), reloadSpeed);
    }

    private void Reset()
    {
        bulletCount.Reset();
    }

    [PunRPC]
    protected void SetBullet_RPC(int viewID, float bulletSpeed, float damage, Vector2 dir, bool isBlockedByBlock)
    {
        GameObject bulletObj = PhotonView.Find(viewID)?.gameObject;
        if (bulletObj == null)
        {
            Debug.LogWarning("총알 오브젝트를 찾을 수 없습니다.");
            return;
        }

        bulletObj.transform.parent = GameObject.Find("BulletManager").transform;

        Bullet b = bulletObj.GetComponent<Bullet>();
        List<string> TargetTags = new List<string>(player.TargetTags);
        if (isBlockedByBlock)
            TargetTags.Add(Tags.Ground);
        else
            TargetTags.Remove(Tags.Ground);

        b.SetTargetTags(TargetTags);

        
    Debug.Log($"TargetTags count after: {b.TargetTags.Count}");
        b.dir = dir;
        b.Speed = bulletSpeed;
        b.Damage = damage;
    }
}
