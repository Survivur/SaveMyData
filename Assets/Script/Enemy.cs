using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public override List<string> TargetTags { get; protected set; } = new List<string> { Tags.Player };
    [SerializeField] float ShootDelay = 1f;
    [Header("Components")]
    [SerializeField, ReadOnly] GameObject Player = null;

    protected override void Start()
    {
        if (nameUI.str == "") nameUI.str = "abs";
        if (nameUI.strTextName == "") nameUI.strTextName = "EnemyName";
        base.Start();

        if (Player == null) Player = GameObject.Find("Player");

        StartCoroutine(ShootForSecond());
    }

    protected override void Update()
    {
        base.Update();
        AimPosition = ((Vector2)(Player.transform.position - transform.position)).normalized;
    }

    IEnumerator ShootForSecond()
    {
        while (true)
        {
            Shoot(Damage, AimPosition);
            yield return new WaitForSeconds(ShootDelay);
        }
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