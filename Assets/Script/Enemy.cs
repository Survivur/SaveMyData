using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] float ShootDelay = 1f;
    [Header("Components")]
    [SerializeField, ReadOnly] GameObject Player = null;

    protected override void Start()
    {
        TargetTags = new List<string> { Tags.Player };
        if (nameUI.str == "") nameUI.str = "abs";
        if (nameUI.strTextName == "") nameUI.strTextName = "EnemyName";
        base.Start();

        if (Player == null) Player = GameObject.Find("Player");

        StartCoroutine(ShootForSecond());
    }

    IEnumerator ShootForSecond()
    {
        while (true)
        {
            // if (Player != null)
            //     //Shoot(Damage, AimDirection);
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