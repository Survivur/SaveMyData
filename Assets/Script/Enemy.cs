using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public override List<string> TargetTags { get; } = new List<string> { Tags.Player };
    [SerializeField, ReadOnly] private string nameText_name = "EnemyName";

    [SerializeField, ReadOnly(true)] private string enemyName = "abs";

    protected override void Start()
    {   
        base.Start();
        nameText = GameObject.Find("Canvas").transform.Find(nameText_name).GetComponent<TextMeshProUGUI>();
        nameText.text = enemyName;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IHittable hitable = other.GetComponent<IHittable>();    
        if (hitable != null)
        {
            hitable?.TakeDamage(Damage, (Vector2)transform.right);
        }
    }
}