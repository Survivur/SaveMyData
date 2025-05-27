using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Character
{
    private PhotonView photonView; // 포톤 서버 관련 추가
    public bool seeRight => sprite.flipX;
    public bool isAnimating => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

    [SerializeField, ReadOnly(true)] private string playerName = "John Wick";
    [SerializeField, ReadOnly(true)] private string nameText_name = "PlayerName";
    
    [SerializeField] private Vector3 shootGap = Vector3.zero;

    [SerializeField, ReadOnly(true)] private float moveSpeed = 1.0f;
    [SerializeField, ReadOnly(true)] private float DashSpeed = 2.0f;
    [SerializeField, ReadOnly(true)] private float DashVelocity = 1f;
    [SerializeField, ReadOnly] private float DashVelocityNormal = 1f;
    [SerializeField, ReadOnly] private int DashCount = 1;
    [SerializeField, ReadOnly(true)] private int DashCountMax = 1;
    [SerializeField, ReadOnly(true)] private float DashCoolDown = 1f;
    [SerializeField, ReadOnly(true)] private float DashDuration = 0.2f;
    [SerializeField, ReadOnly] private bool DashInput = false;

    [SerializeField] private float JumpSpeed = 7f;
    [SerializeField, ReadOnly] private int JumpCount = 1;
    [SerializeField, ReadOnly(true)] private int JumpCountMax = 1;
    [SerializeField, ReadOnly] private bool JumpInput = false;
    [SerializeField, ReadOnly] private bool isJumping = false;

    [SerializeField, ReadOnly] private string bulletPath = "Prefabs/bullet_gun";
    [SerializeField, ReadOnly(true)] private int bulletCountMax = 10;
    [SerializeField, ReadOnly(true)] private int bulletCount = 0;
    [SerializeField] private float reloadSpeed = 1.5f;
    [SerializeField, ReadOnly(true)] private float bulletSpeed = 10f;
    [SerializeField, ReadOnly(true)] private float bulletDelay = 0.3f;
    
    [SerializeField, ReadOnly(true)] private TextMeshProUGUI bulletText = null;

    [SerializeField, ReadOnly] private float ghostDuration = 0.3f;

    [SerializeField, ReadOnly(true)] private Animator animator = null;
    [SerializeField, ReadOnly] private bool needCheckingAnimate = true;

    [SerializeField, ReadOnly(true)] private GameObject UpsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject DownsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject ArmChild = null;

    [SerializeField, ReadOnly] private string ShootKeyCode = "j";

    [Header("Info")]
    [ReadOnly, SerializeField] private float HorizontalInput;

    private bool ReloadCoroutineFlag = false;

    public override List<string> TargetTags { get; } = new List<string>();

    protected override void Start()
    {
        base.Start();
        if (photonView == null) photonView = GetComponent<PhotonView>();
        if (animator == null) animator = GetComponent<Animator>();
        if (bulletText == null) bulletText = GameObject.Find("Canvas").transform.Find("PlayerBulletCount").GetComponent<TextMeshProUGUI>();
        if (nameText == null) nameText = GameObject.Find("Canvas").transform.Find(nameText_name).GetComponent<TextMeshProUGUI>();

        UpsideChild ??= transform.Find("Upside").gameObject;
        DownsideChild ??= transform.Find("Downside").gameObject;
        ArmChild ??= transform.Find("Arm").gameObject;

        playerName = (PhotonNetwork.NickName != "") ? PhotonNetwork.NickName : playerName;
        nameText.text = playerName;

        shootGap = new Vector3(seeRight.BoolToSign(), 0);
        TargetTags.Add(Tags.Enemy);

        if (!photonView.IsMine)  // 자기 플레이어가 아니면 입력 받지 않음
        {
            enabled = false;  // 스크립트 비활성화
            return;
        }

        BulletUpdate();
        BulletTextUpdate();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashInput = true;
        }

        AnimationCheck();
    }

    private void AnimationCheck()
    {
        if (needCheckingAnimate && !isAnimating)
        {
            animator.enabled = false;
            sprite.enabled = false;

            UpsideChild.SetActive(true);
            DownsideChild.SetActive(true);
            ArmChild.SetActive(true);

            needCheckingAnimate = false;
        }
    }


    protected override void FixedUpdate()
    {
        JumpInput = Input.GetAxis("Jump") > 0;
        base.FixedUpdate();
        DashCounting(DashInput && DashCount > 0);

        if (!JumpInput)
        {
            isJumping = false;
        }
    }

    protected override void OnDestroy() {
        
        base.OnDestroy();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground))
        {
            if (collision.transform.position.y < transform.position.y)
                JumpCountReset();
        }
    }

    protected override Vector2 UpdateVelocity(Vector2 velocity)
    {
        HorizontalMovement(ref velocity);
        Dash(ref velocity);
        JumpCheck(ref velocity, JumpInput && JumpCount > 0);

        return velocity;
    }

    /// <summary>
    /// 좌, 우 이동
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 햇는지</returns>
    bool HorizontalMovement(ref Vector2 velocity)
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal") * moveSpeed;
        // 움직이면
        if (HorizontalInput != 0)
        {
            sprite.flipX = HorizontalInput < 0f;
            SpriteRenderer legSpriteRenderer = DownsideChild.GetComponent<SpriteRenderer>();
            legSpriteRenderer.flipX = HorizontalInput > 0f;

            //animator.SetBool("isWalk", true);
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
                StartCoroutine(SpawnGhostCoroutine(ghostDuration));
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

    bool JumpCheck(ref Vector2 velocity, bool condition)
    {
        bool retval = false;
        if (condition && !isJumping && JumpCount > 0)
        {
            retval = Jump(ref velocity);
            JumpCount--;
            isJumping = true;
        }
        return retval;
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

    void JumpCountReset()
    {
        JumpCount = JumpCountMax;
        isJumping = false;
    }

    GameObject CreateBullet(string bulletPath, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(bulletPath);
        if (prefab == null)
        {
            Debug.LogError($"Resources.Load 실패: 경로 {bulletPath}");
            return null;
        }

        if (PhotonNetwork.IsConnected)
        {
            return PhotonNetwork.Instantiate(bulletPath, position, rotation);
        }
        else
        {
            return Instantiate(prefab, position, rotation);
        }
    }

    private void BulletTextUpdate()
    {
        bulletText.text = $"{bulletCount} / {bulletCountMax}";
    }

    private void BulletUpdate()
    {
        bulletCount = bulletCountMax;
    }

    void Shoot(bool isBlockedByBlock = true)
    {
        // bullet count 부분
        if (bulletCount != 0)
        {
            //네트워크를 통해 총알 생성
            GameObject bulletObj = CreateBullet(
                bulletPath,
                transform.position + shootGap * seeRight.BoolToSign(),
                Quaternion.identity);
            bulletObj.transform.parent = GameObject.Find("BulletManager").transform;

            Bullet b = bulletObj.GetComponent<Bullet>();
            if (isBlockedByBlock)
            {
                TargetTags.Add(Tags.Box);
            }
            else
            {
                TargetTags.Remove(Tags.Box);
            }
            b.SetTargetTags(TargetTags);
            b.dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            b.Speed = bulletSpeed;

            bulletCount--;

            if (bulletCount == 0 && !ReloadCoroutineFlag)
                StartCoroutine(Reload());
        }
        BulletTextUpdate();
    }

    private IEnumerator Reload()
    {
        ReloadCoroutineFlag = true;
        yield return new WaitForSeconds(reloadSpeed);
        BulletUpdate();
        BulletTextUpdate();
        ReloadCoroutineFlag = false;
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