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

public abstract class Character : MonoBehaviour, IHittable, IAttackable
{
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

    [SerializeField] protected GameObject HpBar = null;


    [field: SerializeField, ReadOnly] public virtual List<string> TargetTags { get; protected set; } = new List<string>();

    [SerializeField, ReadOnly] new protected Rigidbody2D rigidbody2D;
    [SerializeField, ReadOnly] protected SpriteRenderer spriteRenderer;
    [SerializeField, ReadOnly] protected PhotonView photonView;
    [SerializeField, ReadOnly] protected Vector3 namePosGap = new Vector3(0, 2f, 0);
    [SerializeField, ReadOnly] protected Vector3 HpBarGap = new Vector3(0, 3f, 0);


    protected virtual void Start()
    {
        CodeExtensions.SetIfUnityNull(ref rigidbody2D, GetComponent<Rigidbody2D>());
        CodeExtensions.SetIfUnityNull(ref spriteRenderer, GetComponent<SpriteRenderer>());
        CodeExtensions.SetIfNull(ref photonView, GetComponent<PhotonView>());
        if (nameUI.textObject == null)
        {
            nameUI.textObject = Instantiate(PrefabManager.Instance.NameUI, transform.position, quaternion.identity, GameObjectResource.Instance.Canvas.transform).GetComponent<TextMeshProUGUI>();
            ObjectFollowUI objectFollowUI = nameUI.textObject.GetComponent<ObjectFollowUI>();
            objectFollowUI.TargetObject = gameObject;
            objectFollowUI.Offset = namePosGap;
        }

        if (HpBar == null)
        {
            HpBar = Instantiate(PrefabManager.Instance.HealthBar, transform.position, quaternion.identity, GameObjectResource.Instance.Canvas.transform);
            ObjectFollowUI objectFollowUI = HpBar.GetComponentCached<ObjectFollowUI>();
            objectFollowUI.TargetObject = gameObject;
            objectFollowUI.Offset = HpBarGap;
        }

        nameUI.str = (PhotonNetwork.NickName != "") ? PhotonNetwork.NickName : nameUI.str;
        nameUI.textObject.text = nameUI.str;

        GameObjectResource.Instance.CameraFocusObjects.Add(gameObject);
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



    // GameObject CreateBullet(string bulletPath, Vector3 position, Quaternion rotation)
    // {

    //     if (PhotonNetwork.IsConnected)
    //     {
    //         return PhotonNetwork.Instantiate(bulletPath, position, rotation);
    //     }
    //     else
    //     {
    //         GameObject prefab = Resources.Load<GameObject>(bulletPath);
    //         if (prefab == null)
    //         {
    //             Debug.LogError($"Resources.Load ����: ��� {bulletPath}");
    //             return null;
    //         }
    //         return Instantiate(prefab, position, rotation);
    //     }
    // }

}