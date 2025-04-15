using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, IAttackable, IMoveable
{
    public bool goRight = true;

    public float Damage { get; private set; } = 1f;

    public float Speed { get; set; } = 10f;

    public virtual Vector2 Velocity => new Vector2(Speed * goRight.BoolToSign(), 0);

    public List<string> TargetTags { get; private set; } = null;

    protected void FixedUpdate()
    {
        if (transform == null) return;
        transform.position += (Vector3)Velocity * Time.fixedDeltaTime;

        Debug.Log($"{Speed}, {Velocity}, {Time.fixedDeltaTime}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TargetTags.Contains(collision.tag))
        {
            IHittable hitable = collision.GetComponent<IHittable>();
            hitable?.TakeDamage(Damage, (Vector2)transform.right * new Vector2(goRight.BoolToSign() , 1f));
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
