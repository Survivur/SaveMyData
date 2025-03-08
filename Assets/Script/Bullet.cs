using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, IAttackable, IMoveable
{
    public bool goRight = true;

    public float Damage { get; private set; } = 1f;

    public float Speed { get; protected set; } = 10f;

    public virtual Vector2 Velocity => new Vector3(Speed * (goRight ? 1f : -1f), 0, 0);

    public List<string> TargetTags { get; private set; } = null;

    new private Rigidbody2D rigidbody2D;

    void Start(){
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate()
    {
        IMoveable.UpdateVelocity(rigidbody2D, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TargetTags.Contains(collision.tag))
        {
            IHittable hitable = collision.GetComponent<IHittable>();
            hitable?.TakeDamage(Damage, (Vector2)transform.right);
            DestroyBullet();
        }
    }

    private void OnBecameVisible()
    {
        CancelInvoke(nameof(DestroyBullet));
    }

    void OnBecameInvisible()
    {
        // 3초 뒤에 총알을 파괴합니다.
        Invoke(nameof(DestroyBullet), 3f);
    }
    
    public void SetTargetTags(List<string> targetTags)
    {
        TargetTags ??= new List<string>(targetTags);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
