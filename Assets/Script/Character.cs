using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct NameUI<T>
{
    [Header("Options")]
    [SerializeField, ReadOnly(true)] public T str;
    [SerializeField, ReadOnly(true)] public TextMeshProUGUI textName;
    [SerializeField, ReadOnly(true)] public string strTextName;
}

public abstract class Character : MonoBehaviour, IHittable, IAttackable, IMoveable
{
    [SerializeField, ShowPropertyInInspector]
    public float Speed { get; protected set; } = 5f;

    public float Damage { get; protected set; } = 1f;

    public float MaxHealth { get; protected set; } = 10f;
    public float Health { get; protected set; } = 10f;

    [SerializeField] protected NameUI<string> nameUI = new NameUI<string>
    {
        str = "Character",
        strTextName = "Character"
    };

    public virtual List<string> TargetTags { get; } = new List<string>();

    new protected Rigidbody2D rigidbody2D;
    protected SpriteRenderer sprite;

    public virtual Vector2 Velocity => this.VelocityDefault(rigidbody2D);

    [SerializeField] private Vector3 namePosGap = new Vector3(0, 2f, 0);

    protected virtual void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        //if (nameUI.str == null) nameText = GameObject.Find("Canvas").transform.Find(nameText_name).GetComponent<TextMeshProUGUI>();


        GameObjectResource.Instance.CameraFocusObjects.Add(gameObject);
    }

    protected virtual void Update()
    {
        //nameText.transform.position = Camera.main.WorldToScreenPoint(transform.position + namePosGap);
    }

    protected virtual void FixedUpdate()
    {
        rigidbody2D.linearVelocity = UpdateVelocity(rigidbody2D.linearVelocity);
    }

    protected virtual void OnDestroy()
    {
        GameObjectResource.Instance?.CameraFocusObjects.Remove(gameObject);
        //Destroy(nameText);
    }

    /// <summary>
    /// FixedUpdate에서 실행된다.
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
        // 데미지를 최대 체력만큼만 받을 수 있게합니다.
        Health -= Mathf.Min(dmg, Health);
        if (IHittable.CheckDead(this))
        {
            Destroy(gameObject);
        }
    }
}