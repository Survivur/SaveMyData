using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct NameUI
{
    [Header("Options")]
    [SerializeField, ReadOnly(true)] public string str;
    [SerializeField, ReadOnly(true)] public TextMeshProUGUI textObject;
    [SerializeField, ReadOnly(true)] public string strTextName;
}

public abstract class Character : MonoBehaviour, IHittable, IAttackable, IMoveable, IShootable
{
    [field: SerializeField] public float Speed { get; protected set; } = 5f;
    [field: SerializeField] public float Damage { get; protected set; } = 1f;

    [field: SerializeField] public float MaxHealth { get; protected set; } = 10f;
    [field: SerializeField, ReadOnly] public float Health { get; protected set; } = 10f;

    [field: SerializeField, ReadOnly] public Vector2 AimDirection { get; protected set; } = Vector2.zero;

    [SerializeField] protected NameUI nameUI = new NameUI
    {
        str = "",
        textObject = null,
        strTextName = ""
    };
    [SerializeField] protected float bulletSpeed = 10f;

    [field: SerializeField, ReadOnly] public virtual List<string> TargetTags { get; protected set; } = new List<string>();

    [SerializeField, ReadOnly] new protected Rigidbody2D rigidbody2D;
    [SerializeField, ReadOnly] protected SpriteRenderer spriteRenderer;
    [SerializeField, ReadOnly] protected PhotonView photonView;
    [SerializeField, ReadOnly] protected Vector3 namePosGap = new Vector3(0, 2f, 0);

    public virtual Vector2 Velocity => this.VelocityDefault(rigidbody2D);

    protected virtual void Start()
    {
        CodeExtensions.SetIfUnityNull(ref rigidbody2D, GetComponent<Rigidbody2D>());
        CodeExtensions.SetIfUnityNull(ref spriteRenderer, GetComponent<SpriteRenderer>());

        CodeExtensions.SetIfNull(ref photonView, GetComponent<PhotonView>());
        if (nameUI.textObject == null)
        {
            nameUI.textObject = Instantiate(PrefabManager.Instance.NameUI, transform.position, quaternion.identity, GameObjectResource.Instance.Canvas.transform).GetComponent<TextMeshProUGUI>();
            ObjectFollowUI objectFollowUI =  nameUI.textObject.GetComponent<ObjectFollowUI>();
            objectFollowUI.TargetObject = gameObject;
            objectFollowUI.Offset = namePosGap;
        }

        nameUI.str = (PhotonNetwork.NickName != "") ? PhotonNetwork.NickName : nameUI.str;
        nameUI.textObject.text = nameUI.str;

        GameObjectResource.Instance.CameraFocusObjects.Add(gameObject);
    }

    protected virtual void Update()
    {
        AimDirection = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;
    }

    protected virtual void FixedUpdate()
    {
        rigidbody2D.linearVelocity = UpdateVelocity(rigidbody2D.linearVelocity);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(AimDirection);
    }
    else
    {
        AimDirection = (Vector2)stream.ReceiveNext();
    }
}

    protected virtual void OnDestroy()
    {
        GameObjectResource.Instance?.CameraFocusObjects.Remove(gameObject);
        Destroy(nameUI.textObject);
    }

    /// <summary>
    /// FixedUpdate���� ����ȴ�.
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns></returns>
    protected virtual Vector2 UpdateVelocity(Vector2 velocity)
    {
        return velocity;
    }

    public virtual void TakeDamage(float dmg, Vector2 dir)
    {
        IHittable.ApplyKnockback(rigidbody2D, dmg, dir);
        Debug.Log($"{name} takes {dmg} damage. now hp is {Health}.");
        // �������� �ִ� ü�¸�ŭ�� ���� �� �ְ��մϴ�.
        Health -= Mathf.Min(dmg, Health);
        if (IHittable.CheckDead(this))
        {
            Destroy(gameObject);
        }
    }

    public virtual void TakeDamage(Bullet bullet, Vector2 dir)
    {
        IHittable.ApplyKnockback(rigidbody2D, bullet.Damage, dir);
        //Debug.Log($"{name} takes {bullet.Damage} damage. now hp is {Health}.");
        // �������� �ִ� ü�¸�ŭ�� ���� �� �ְ��մϴ�.
        Health -= Mathf.Min(bullet.Damage, Health);
        if (IHittable.CheckDead(this))
        {
            Destroy(gameObject);
        }
    }

    //[PunRPC]
    public virtual void Shoot(float? damage = null, Vector2? dir = null, bool isBlockedByBlock = true)
    {
        if (dir == null)
        {
            dir = AimDirection;
        }
        if (damage == null)
        {
            damage = Damage;
        }

        //��Ʈ��ũ�� ���� �Ѿ� ����
        GameObject bulletObj = CreateBullet(
            ObjectPath.Bullet,
            transform.position + (Vector3)dir,
            Quaternion.identity);
        bulletObj.transform.parent = GameObject.Find("BulletManager").transform;

        Bullet b = bulletObj.GetComponent<Bullet>();
        if (isBlockedByBlock)
        {
            TargetTags.Add(Tags.Box);
        }
        else
        {
            TargetTags.Remove(Tags.Box);
        }
        b.SetTargetTags(TargetTags);
        b.dir = dir ?? Vector2.zero;
        b.Speed = bulletSpeed;
        b.Damage = damage ?? 0;
    }


    //[PunRPC]
    public virtual void Shoot(Bullet bullet)
    {
        //��Ʈ��ũ�� ���� �Ѿ� ����
        GameObject bulletObj = CreateBullet(
            ObjectPath.Bullet,
            transform.position + (Vector3)bullet.dir,
            Quaternion.identity);
        bulletObj.transform.parent = GameObject.Find("BulletManager").transform;

        Bullet b = bulletObj.GetComponent<Bullet>();
        b.SetTargetTags(TargetTags);
        b.dir = bullet.dir;
        b.Speed = bullet.Speed;
        b.Damage = bullet.Damage;
    }

    GameObject CreateBullet(string bulletPath, Vector3 position, Quaternion rotation)
    {

        if (PhotonNetwork.IsConnected)
        {
            return PhotonNetwork.Instantiate(bulletPath, position, rotation);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(bulletPath);
            if (prefab == null)
            {
                Debug.LogError($"Resources.Load ����: ��� {bulletPath}");
                return null;
            }
            return Instantiate(prefab, position, rotation);
        }
    }
}