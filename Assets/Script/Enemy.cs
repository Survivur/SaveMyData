using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public override List<string> TargetTags { get; } = new List<string> { Tags.Player };

    void OnTriggerEnter2D(Collider2D other)
    {
        IHittable hitable = other.GetComponent<IHittable>();
        if (hitable != null)
        {
            hitable?.TakeDamage(Damage, (Vector2)transform.right);
        }
    }
}