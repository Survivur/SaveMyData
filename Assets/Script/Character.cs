using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IHittable, IAttackable, IMoveable
{
    public float Speed { get; protected set; } = 5f;

    public float Damage { get; protected set; } = 1f;

    public float MaxHealth { get; protected set; } = 10f;
    public float Health { get; protected set; } = 10f;

    public virtual List<string> TargetTags { get; } = new List<string>();

    new protected Rigidbody2D rigidbody2D;
    protected SpriteRenderer sprite;

    public virtual Vector2 Velocity => IMoveable.VelocityDefault(rigidbody2D, this);

    protected virtual void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        IMoveable.UpdateVelocity(rigidbody2D, this);
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