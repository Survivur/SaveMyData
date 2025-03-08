using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{    
    public float JumpSpeed = 3f;
    public int JumpCount = 1;
    public int JumpCountMax = 1;
    private GameObject bullet;

    public override List<string> TargetTags { get; } = new List<string>();

    bool JumpInput = false;

    protected override void Start()
    {
        base.Start();
        bullet = Resources.Load<GameObject>("Prefabs/bullet_gun");
        TargetTags.Add(Tags.Enemy);
    }

    protected void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            Shoot();
        }
    }
    
    public override Vector2 Velocity
    { 
        get
        {
            float HorizontalMovement = Input.GetAxis("Horizontal") * Speed;
            return Jump(new Vector2(HorizontalMovement, rigidbody2D.linearVelocityY));
        }
    }

    protected override void FixedUpdate()
    {
        JumpInput = Input.GetAxis("Jump") > 0;
        base.FixedUpdate();
        JumpCounting();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            JumpCountReset();
        }
    }

    Vector2 Jump(Vector2 velocity)
    {
        velocity.y.SetIfTrue(JumpInput && JumpCount > 0, JumpSpeed);
        return velocity;
    }

    void JumpCounting()
    {
        JumpCount.SetIfTrue(JumpInput && JumpCount > 0, JumpCount - 1);
    }

    void JumpCountReset()
    {
        JumpCount = JumpCountMax;
    }

    void Shoot()
    {
        Bullet b = Instantiate(bullet, transform.position, Quaternion.identity, ObjectManager.BulletManager.transform).GetComponent<Bullet>();
        b.SetTargetTags(TargetTags);
    }
}