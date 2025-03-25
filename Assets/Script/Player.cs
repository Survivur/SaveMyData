 using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public bool seeright => sprite.flipX;
    
    [SerializeField, ReadOnly(true)] private float DashSpeed = 2.0f;
    [SerializeField, ReadOnly(true)] private float DashVelocity = 1f;
    [SerializeField, ReadOnly] private int DashCount = 1;
    [SerializeField, ReadOnly(true)] private int DashCountMax = 1;
    [SerializeField, ReadOnly(true)] private float DashCoolDown = 1f;
    [SerializeField, ReadOnly(true)] private float DashDuration = 0.2f;
    [SerializeField, ReadOnly] private bool DashInput = false;

    [SerializeField, ReadOnly(true)] private float JumpSpeed = 7f;
    [SerializeField, ReadOnly] private int JumpCount = 1;
    [SerializeField, ReadOnly(true)] private int JumpCountMax = 1;
    [SerializeField, ReadOnly] private bool JumpInput = false;

    [SerializeField, ReadOnly] private GameObject bullet;

    public override List<string> TargetTags { get; } = new List<string>();

    public override Vector2 Velocity
    {
        get
        {
            Vector2 velocity = rigidbody2D.linearVelocity;

            HorizontalMovement(ref velocity);
            Dash(ref velocity);
            Jump(ref velocity);
                
            return velocity;
        }
    }

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
        DashInput.SetIfTrue(Input.GetKeyDown(KeyCode.LeftShift), true);
    }

    protected override void FixedUpdate()
    {
        JumpInput = Input.GetAxis("Jump") > 0;
        base.FixedUpdate();
        JumpCounting(JumpInput && JumpCount > 0);
        DashCounting(DashInput && DashCount > 0);
        DashInput = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            JumpCountReset();
        }
    }

    /// <summary>
    /// 좌, 우 이동
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 햇는지</returns>
    bool HorizontalMovement(ref Vector2 velocity)
    {
        float HorizontalInput = Input.GetAxis("Horizontal");

        FlipSpriteBasedOnInput(HorizontalInput);

        velocity.x = HorizontalInput * Speed;
        return velocity.x != 0;
    }

    private void FlipSpriteBasedOnInput(float horizontal)
    {
        if (horizontal != 0f)
        {
            sprite.flipX = horizontal > 0f;
        }
    }

    /// <summary>
    /// 대쉬
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 했는지</returns>
    bool Dash(ref Vector2 velocity)
    {
        DashVelocity.SetIfTrue(DashInput && DashCount > 0, DashSpeed);

        if (DashVelocity > 1f)
        {
            velocity.x = seeright.BoolToSign() * Speed * DashVelocity;
            DashVelocity -= Time.fixedDeltaTime * Mathf.Abs(DashSpeed - 1f) / DashDuration;
        }
        else
        {
            DashVelocity = 1f;
        }
        return velocity.x != 0;
    }

    void DashCounting(bool condition)
    {
        DashCount.SetIfTrue(condition, DashCount - 1);
        if (DashCount == 0 && !IsInvoking(nameof(DashCountReset)))
        {
            Invoke(nameof(DashCountReset), DashCoolDown);
        }
    }

    void DashCountReset()
    {
        DashCount = DashCountMax;
    }

    /// <summary>
    /// 점프
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 했는지</returns>
    bool Jump(ref Vector2 velocity)
    {
        velocity.y.SetIfTrue(JumpInput && JumpCount > 0, JumpSpeed);

        return velocity.y != 0;
    }

    void JumpCounting(bool condition)
    {
        JumpCount.SetIfTrue(condition, JumpCount - 1);
    }

    void JumpCountReset()
    {
        JumpCount = JumpCountMax;
    }

    void Shoot()
    {
        Bullet b = Instantiate(bullet, transform.position, Quaternion.identity, ObjectManager.BulletManager.transform).GetComponent<Bullet>();
        b.SetTargetTags(TargetTags);
        b.goRight = seeright;
    }
}