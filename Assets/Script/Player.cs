using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct DashData
{
    [Header("Options")]
    [SerializeField] public float startSpeed;
    [SerializeField] public float minSpeed;
    [SerializeField] public int maxCount;
    [SerializeField] public float coolDown;
    [SerializeField] public float delay;

    [Header("Information")]
    [SerializeField, ReadOnly] public bool goRight;
    [SerializeField, ReadOnly] public bool isKeyDown;
    [SerializeField, ReadOnly] public float velocityX;
    [SerializeField, ReadOnly] public int count;
}

[System.Serializable]
public struct JumpData
{
    [Header("Options")]
    [SerializeField] public float speed;
    [SerializeField] public int countMax;
    [SerializeField] public float coolDown;

    [Header("Information")]
    [SerializeField, ReadOnly] public int count;
    [SerializeField, ReadOnly] public bool isKeyDown;
    [SerializeField, ReadOnly] public bool isJumping;
}

[System.Serializable]
public struct ShootInfo
{
    [SerializeField, ReadOnly(true)] public int bulletCountMax;
    [SerializeField, ReadOnly(true)] public int bulletCount;
    [SerializeField] public float reloadSpeed;
    [SerializeField, ReadOnly(true)] public float bulletSpeed;
    [SerializeField, ReadOnly(true)] public float bulletDelay;
    [SerializeField, ReadOnly(true)] public float shootGap;
}

[System.Serializable]
public struct AnimationFlags
{
    [SerializeField, ReadOnly] public bool needCheckingAnimate;
}

[System.Serializable]
public struct ObjectRefs
{
    [SerializeField, ReadOnly(true)] public TextMeshProUGUI bulletText;
    [SerializeField, ReadOnly(true)] public Animator animator;
    [SerializeField, ReadOnly(true)] public GameObject upsideChild;
    [SerializeField, ReadOnly(true)] public GameObject downsideChild;
    [SerializeField, ReadOnly(true)] public GameObject armChild;
    [SerializeField, ReadOnly(true)] public SpriteRenderer upsideChildSprite;
}

public class Player : Character
{
    public bool seeRight => UpsideChildSprite.flipX;
    public bool isAnimating => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

    [SerializeField] private DashData dash = new DashData {
        startSpeed = 2f,
        minSpeed = 1f,
        maxCount = 1,
        coolDown = 1f,
        delay = 0.2f,

        goRight = false,
        isKeyDown = false,
        velocityX = 1f,
        count = 1,
    };

    [SerializeField] private JumpData jump = new JumpData
    {
        speed = 7f,
        countMax = 1,
        count = 1,
        isKeyDown = false,
        isJumping = false
    };


    [SerializeField, ReadOnly(true)] protected int bulletCountMax = 10;
    [SerializeField, ReadOnly(true)] protected int bulletCount = 0;
    [SerializeField, ReadOnly(true)] protected float reloadSpeed = 1.5f;
    [SerializeField, ReadOnly(true)] protected float bulletDelay = 0.3f;
    [SerializeField, ReadOnly(true)] protected float ShootGap = 1f;

    [Header("ghost", order = 1)]
    [SerializeField, ReadOnly(true)] private float ghostDuration = 0.3f;

    [Header("Animation", order = 0)]
    [SerializeField, ReadOnly] private bool needCheckingAnimate = true;

    [Header("Object & Components", order = 0)]
    [SerializeField, ReadOnly(true)] private TextMeshProUGUI bulletText = null;
    [SerializeField, ReadOnly(true)] private Animator animator = null;
    [SerializeField, ReadOnly(true)] private GameObject UpsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject DownsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject ArmChild = null;
    [SerializeField, ReadOnly(true)] private SpriteRenderer UpsideChildSprite = null;

    [Header("Info", order = 0)]
    [SerializeField, ReadOnly] private float HorizontalInput;

    private bool ReloadCoroutineFlag = false;

    public override List<string> TargetTags { get; protected set; } = new List<string>();

    protected override void Start()
    {
        if (nameUI.str == "") nameUI.str = "John Wick"; 
        if (nameUI.strTextName == "") nameUI.strTextName = "PlayerName";
        base.Start();

        if (animator == null) animator = GetComponent<Animator>();
        if (bulletText == null) bulletText = GameObject.Find("Canvas").transform.Find("PlayerBulletCount").GetComponent<TextMeshProUGUI>();

        if (UpsideChild == null) UpsideChild = transform.Find("Upside").gameObject;
        if (DownsideChild == null) DownsideChild = transform.Find("Downside").gameObject;
        if (ArmChild == null) ArmChild = transform.Find("Arm").gameObject;

        if (UpsideChildSprite == null) UpsideChildSprite = UpsideChild.GetComponent<SpriteRenderer>();
        UpsideChild.SetActive(false);
        DownsideChild.SetActive(false);
        ArmChild.SetActive(false);

        TargetTags.Add(Tags.Enemy);

        if (!photonView.IsMine)  // 자기 플레이어가 아니면 입력 받지 않음
        {
            enabled = false;  // 스크립트 비활성화
            return;
        }

        JumpCountReset();
        DashCountReset();
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
            dash.isKeyDown = true;
        }

        AnimationCheck();
    }

    protected override void FixedUpdate()
    {
        jump.isKeyDown = Input.GetAxis("Jump") > 0;
        base.FixedUpdate();
        if (dash.isKeyDown)
        {
            DashCounting();
            dash.isKeyDown = false;
        }

        if (!jump.isKeyDown)
        {
            jump.isJumping = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground) | collision.gameObject.CompareTag(Tags.Box))
        {
            if (collision.transform.position.y < transform.position.y)
                JumpCountReset();
        }
    }

    protected override Vector2 UpdateVelocity(Vector2 velocity)
    {
        HorizontalMovement(ref velocity);
        CalcurateDashVelocity(ref velocity);
        JumpCheck(ref velocity, jump.isKeyDown && jump.count > 0);

        return velocity;
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

    /// <summary>
    /// 좌, 우 이동
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 햇는지</returns>
    bool HorizontalMovement(ref Vector2 velocity)
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        // 움직이면
        if (HorizontalInput != 0)
        {
            sprite.flipX = HorizontalInput < 0f;
            SpriteRenderer legSpriteRenderer = DownsideChild.GetComponent<SpriteRenderer>();
            legSpriteRenderer.flipX = HorizontalInput > 0f;

            //animator.SetBool("isWalk", true);
        }

        velocity.x = HorizontalInput * Speed ;

        return velocity.x != 0;
    }

    /// <summary>
    /// 대쉬
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>이동 했는지</returns>
    bool CalcurateDashVelocity(ref Vector2 velocity)
    {
        if (dash.isKeyDown && dash.count > 0)
        {
            dash.velocityX = dash.startSpeed;
            dash.goRight = seeRight;
        }

        if (dash.velocityX > dash.minSpeed)
        {
            velocity.x = dash.goRight.BoolToSign() * Speed * dash.velocityX;
            dash.velocityX -= Time.fixedDeltaTime * Mathf.Abs(dash.startSpeed - 1f) / dash.delay;

            if (!IsInvoking(nameof(SpawnGhostCoroutine)))
            {
                StartCoroutine(SpawnGhostCoroutine(ghostDuration));
            }
        }
        else
        {
            dash.velocityX = dash.minSpeed;
        }
        return velocity.x != 0;
    }

    void DashCounting()
    {
        dash.count--;
        if (dash.count == 0 && !IsInvoking(nameof(DashCountReset)))
        {
            Invoke(nameof(DashCountReset), dash.coolDown);
        }
    }

    void DashCountReset()
    {
        dash.count = dash.maxCount;
    }

    bool JumpCheck(ref Vector2 velocity, bool condition)
    {
        bool retval = false;
        if (condition && !jump.isJumping && jump.count > 0)
        {
            retval = CalcurateJumpVelocity(ref velocity);
            jump.count--;
            jump.isJumping = true;
            Invoke(nameof(JumpAble), jump.coolDown);
        }
        return retval;
    }

    /*************  ? Windsurf Command ?  *************/
        /// <summary>
        /// 점프
        /// </summary>
        /// <param name="velocity">점프한 후의 속도</param>
        /// <returns>점프했는지</returns>
        /// <remarks>
        /// 점프 제한이 걸려 있지 않을 경우 점프 속도를 velocity에 저장하고, 제한을 걸어 준다.
        /// </remarks>
    /*******  c9744dbd-065d-493a-b0d8-42f446fd130d  *******/    
    bool CalcurateJumpVelocity(ref Vector2 velocity)
    {
        if (!jump.isJumping && jump.isKeyDown && jump.count > 0)
        {
            velocity.y = jump.speed;
        }

        return velocity.y != 0;
    }

    void JumpAble()
    {
        jump.isJumping = false;
    }

    void JumpCountReset()
    {
        jump.count = jump.countMax;
        jump.isJumping = false;
    }

    private void BulletTextUpdate()
    {
        bulletText.text = $"{bulletCount} / {bulletCountMax}";
    }

    private void BulletUpdate()
    {
        bulletCount = bulletCountMax;
    }

    public override void Shoot(float? damage = null, Vector2? dir = null, bool isBlockedByBlock = true)
    {
        // bullet count 부분
        if (bulletCount != 0)
        {
            base.Shoot(damage, dir, isBlockedByBlock);

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
        ghost.transform.localScale = transform.localScale;
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