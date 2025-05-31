using System;
using UnityEngine;


public interface IHittable
{
    public float Health { get; }
    void TakeDamage(float damage, Vector2 direction);
    void TakeDamage(Bullet bullet, Vector2 direction);

    public static bool CheckDead(IHittable hittable)
    {
        if (hittable == null)
        {
            Debug.LogError("Hittable is null");
            return false;
        }
        return hittable.Health <= 0f;
    }


    public static void ApplyKnockback(Rigidbody2D rigidbody2D, float force, Vector2 direction)
    {
        if (rigidbody2D == null) return;
        rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
    }
}