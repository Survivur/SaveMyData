using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private PhotonView photonView; // 포톤 서버 관련 추가
    public bool seeRight => sprite.flipX;
    
    [SerializeField, ReadOnly(true)] private Vector3 shootGap = Vector3.zero;

    [SerializeField, ReadOnly(true)] private float moveSpeed = 1.0f;
    [SerializeField, ReadOnly(true)] private float DashSpeed = 2.0f;
    [SerializeField, ReadOnly(true)] private float DashVelocity = 1f;
    [SerializeField, ReadOnly] private float DashVelocityNormal = 1f;
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

    [SerializeField, ReadOnly] private Animator animator;

    [SerializeField, ReadOnly] string ShootKeyCode = "j";

    public override List<string> TargetTags { get; } = new List<string>();

    protected override void Start()
    {
        base.Start();
        photonView = GetComponent<PhotonView>();
        bullet = Resources.Load<GameObject>("Prefabs/bullet_gun");
        animator = GetComponent<Animator>();

        shootGap = new Vector3(seeRight.BoolToSign(), 0);
        TargetTags.Add(Tags.Enemy);

        if (!photonView.IsMine)  // 자기 플레이어가 아니면 입력 받니 않음
        {
            enabled = false;  // 스크립트 비활성화
            return;
        }
    }

    protected void Update()
    {
        if (Input.GetKeyDown(ShootKeyCode))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashInput = true;
        }
    }

    protected override void FixedUpdate()
    {
        JumpInput = Input.GetAxis("Jump") > 0;
        base.FixedUpdate();
        JumpCounting(JumpInput && JumpCount > 0);
        DashCounting(DashInput && DashCount > 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            JumpCountReset();
        }
    }

    protected override Vector2 UpdateVelocity(Vector2 velocity)
    {
        HorizontalMovement(ref velocity);
        Dash(ref velocity);
        Jump(ref velocity);

        return velocity;
    }

    /// <summary>
    /// 좌, 우 이동
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 햇는지</returns>
    bool HorizontalMovement(ref Vector2 velocity)
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal") * moveSpeed;
        // 움직이면
        if (HorizontalInput != 0)
        {   
            sprite.flipX = HorizontalInput < 0f;

            animator.SetBool("isWalk", true);
        }
        
        velocity.x = HorizontalInput * Speed;

        return velocity.x != 0;
    }

    /// <summary>
    /// 대쉬
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 했는지</returns>
    bool Dash(ref Vector2 velocity)
    {
        DashVelocity.SetIfTrue(DashInput && DashCount > 0, DashSpeed);

        if (DashVelocity > DashVelocityNormal)
        {
            velocity.x = seeRight.BoolToSign() * Speed * DashVelocity;
            DashVelocity -= Time.fixedDeltaTime * Mathf.Abs(DashSpeed - 1f) / DashDuration;

            if (!IsInvoking(nameof(SpawnGhostCoroutine)))
            {
                StartCoroutine(SpawnGhostCoroutine(0.1f));
            }
        }
        else
        {
            DashVelocity = DashVelocityNormal;
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
        DashInput = false;
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
        //네트워크를 통해 총알 생성
        GameObject bulletObj = PhotonNetwork.Instantiate(
            "bullet_gun",
            transform.position,
            Quaternion.identity);
        Bullet b = Instantiate(bullet, transform.position + shootGap, Quaternion.identity, ObjectManager.BulletManager.transform).GetComponent<Bullet>();
        b.SetTargetTags(TargetTags);
        b.goRight = seeRight;
    }

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(PrefabManager.Instance.Ghost, 
            transform.position - new Vector3(0, 0, 1f), 
            Quaternion.identity, 
            GameObjectResource.Instance.GhostManager.transform);

        SpriteRenderer spriteRenderer = ghost.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite.sprite;
        spriteRenderer.flipX = sprite.flipX;
        spriteRenderer.color = new Color(1, 1, 1, 0.2f);

    }

    IEnumerator SpawnGhostCoroutine(float delay)
    {
        SpawnGhost();
        yield return new WaitForSeconds(delay);
    }
}