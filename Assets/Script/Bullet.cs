using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, IAttackable, IMoveable
{
    [SerializeField, ReadOnly] public Vector2 dir = Vector2.zero;
        
    [SerializeField, ReadOnly(true)] private float _damage = 1f;
    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }

    [SerializeField, ReadOnly(true)] private float _speed = 10f;
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    
    public virtual Vector2 Velocity => dir * Speed;

    [SerializeField, ReadOnly] private List<string> _targetTags = null;
    public List<string> TargetTags
    {
        get => _targetTags;
        private set => _targetTags = value;
    }

    protected void FixedUpdate()
    {
        if (transform == null) return;
        transform.position += (Vector3)Velocity * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        //Debug.Log($"{Speed}, {Velocity}, {Time.fixedDeltaTime}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TargetTags.Contains(collision.tag))
        {
            IHittable hitable = collision.GetComponent<IHittable>();
            hitable?.TakeDamage(this, (Vector2)transform.right * new Vector2(dir.x , 1f));
            DestroyBullet();
        }
    }

    private void OnBecameVisible()
    {
        CancelInvoke(nameof(DestroyBullet));
    }

    void OnBecameInvisible()
    {
        // 3�� �ڿ� �Ѿ��� �ı��մϴ�.
        Invoke(nameof(DestroyBullet), 3f);
    }
    
    public void SetTargetTags(List<string> targetTags)
    {
        TargetTags = new List<string>(targetTags);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
