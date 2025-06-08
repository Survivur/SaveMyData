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
    [SerializeField] public Counter counter;
    [SerializeField] public float startSpeed;
    [SerializeField] public float minSpeed;
    [SerializeField] public float coolDown;
    [SerializeField] public float delay;

    [Header("Information")]
    [SerializeField, ReadOnly] public bool goRight;
    [SerializeField, ReadOnly] public bool isKeyDown;
    [SerializeField, ReadOnly] public float velocityX;
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
public struct GunData
{
    [Header("Options")]
    [SerializeField] public Counter bulletCount;
    [SerializeField] public float reloadSpeed;

    [Header("Information")]
    [SerializeField, ReadOnly(true)] public float bulletDelay;
    [SerializeField, ReadOnly(true)] public float shootGap;
}

[System.Serializable]
public struct MoveData
{
    [Header("Options")]
    [SerializeField] public Counter bulletCount;
    [SerializeField] public float reloadSpeed;

    [Header("Information")]
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

public class MoveableCharacter : Character
{
    public bool seeRight => DownsideChildSprite.flipX;
    public bool isAnimating => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

    [SerializeField] protected DashData dash = new DashData {
        counter = new Counter(1),
        startSpeed = 2f,
        minSpeed = 1f,
        coolDown = 1f,
        delay = 0.2f,

        goRight = false,
        isKeyDown = false,
        velocityX = 1f,
    };

    [SerializeField] protected JumpData jump = new JumpData
    {
        speed = 7f,
        countMax = 1,
        count = 1,
        isKeyDown = false,
        isJumping = false
    };


    [SerializeField] protected GunData gun = new GunData
    {
        bulletCount = new Counter(10),
        reloadSpeed = 1.5f,
        bulletDelay = 0.3f,
        shootGap = 1f
    };

    public string BulletText => $"{gun.bulletCount}/{gun.bulletCount.Max}";
    
    // [SerializeField, ReadOnly(true)] protected int bulletCountMax = 10;
    // [SerializeField, ReadOnly(true)] protected int bulletCount = 0;
    // [SerializeField, ReadOnly(true)] protected float reloadSpeed = 1.5f;
    // [SerializeField, ReadOnly(true)] protected float bulletDelay = 0.3f;
    // [SerializeField, ReadOnly(true)] protected float ShootGap = 1f;

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
    [SerializeField, ReadOnly(true)] private SpriteRenderer DownsideChildSprite = null;

    [Header("Info", order = 0)]
    [SerializeField, ReadOnly] protected float HorizontalInput = 0f;
    
    private bool ReloadCoroutineFlag = false;

    public override List<string> TargetTags { get; protected set; } = new List<string>();

    protected override void Start()
    {
        CodeExtensions.SetIfNullOrEmpty(ref nameUI.str, "John Wick");
        CodeExtensions.SetIfNullOrEmpty(ref nameUI.strTextName, "PlayerName");

        base.Start();
        
        CodeExtensions.SetIfUnityNull(ref animator, GetComponent<Animator>());
        CodeExtensions.SetIfUnityNull(ref bulletText, GameObject.Find("Canvas").transform.Find("PlayerBulletCount").GetComponent<TextMeshProUGUI>());

        CodeExtensions.SetIfUnityNull(ref UpsideChild,  transform.Find("Upside").gameObject);
        CodeExtensions.SetIfUnityNull(ref DownsideChild, transform.Find("Downside").gameObject);
        CodeExtensions.SetIfUnityNull(ref ArmChild, transform.Find("Arm").gameObject);

        CodeExtensions.SetIfUnityNull(ref DownsideChildSprite, DownsideChild.GetComponent<SpriteRenderer>());

        UpsideChild.SetActive(false);
        DownsideChild.SetActive(false);
        ArmChild.SetActive(false);

        TargetTags.Add(Tags.Enemy);

        JumpCountReset();
        dash.counter.Reset();
        gun.bulletCount.Reset();
    }


    protected override void Update()
    {
        base.Update();
        AnimationCheck();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (dash.isKeyDown)
        {
            DashCounting();
            dash.isKeyDown = false;
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
        if (jump.isKeyDown && !jump.isJumping && jump.count > 0)
        {
            JumpCheck(ref velocity);
            jump.isKeyDown = false;
        }

        return velocity;
    }

    private void AnimationCheck()
    {
        if (needCheckingAnimate && !isAnimating)
        {
            animator.enabled = false;
            spriteRenderer.enabled = false;

            UpsideChild.SetActive(true);
            DownsideChild.SetActive(true);
            ArmChild.SetActive(true);

            needCheckingAnimate = false;
        }
    }

    /// <summary>
    /// ??, ?? ???
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>??? ?????</returns>
    bool HorizontalMovement(ref Vector2 velocity)
    {
        // ???????
        if (HorizontalInput != 0)
        {
            spriteRenderer.flipX = HorizontalInput < 0f;
            SpriteRenderer legSpriteRenderer = DownsideChild.GetComponent<SpriteRenderer>();
            legSpriteRenderer.flipX = HorizontalInput > 0f;
        }

        velocity.x = HorizontalInput * Speed ;

        return velocity.x != 0;
    }

    /// <summary>
    /// ?��
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>??? ?????</returns>
    bool CalcurateDashVelocity(ref Vector2 velocity)
    {
        if (dash.isKeyDown && dash.counter > 0)
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
        if (dash.counter.Counting())
        {
            StartCoroutine(dash.counter.CountResetCoroutine(dash.coolDown));
            Invoke(nameof(dash.counter.Reset), dash.coolDown);
        }
    }


    bool JumpCheck(ref Vector2 velocity)
    {
        jump.count--;
        jump.isJumping = true;
        Invoke(nameof(JumpAble), jump.coolDown);
        return CalcurateJumpVelocity(ref velocity);
    }

    bool CalcurateJumpVelocity(ref Vector2 velocity)
    {
        velocity.y = jump.speed;

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
    
    public override void Shoot(float? damage = null, Vector2? dir = null, bool isBlockedByBlock = true)
    {
        // bullet count ?��?
        if (gun.bulletCount != 0)
        {
            base.Shoot(damage, dir, isBlockedByBlock);

            gun.bulletCount.Count--;

            if (gun.bulletCount == 0 && !ReloadCoroutineFlag)
                StartCoroutine(Reload());
        }
        //BulletTextUpdate();
    }

    [PunRPC]
    public void RPC_Dash()
    {
        dash.isKeyDown = true;
    }

    [PunRPC]
    public void RPC_Jump()
    {
        jump.isKeyDown = true;
    }

    [PunRPC]
    public void RPC_SetAim()
    {
        AimDirection = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;
    }

    private IEnumerator Reload()
    {
        ReloadCoroutineFlag = true;
        yield return new WaitForSeconds(gun.reloadSpeed);
        gun.bulletCount.Reset();
        //BulletTextUpdate();
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
        spriteRenderer.sprite = this.spriteRenderer.sprite;
        spriteRenderer.flipX = this.spriteRenderer.flipX;
        spriteRenderer.color = new Color(1, 1, 1, 0.2f);
    }

    IEnumerator SpawnGhostCoroutine(float delay)
    {
        SpawnGhost();
        yield return new WaitForSeconds(delay);
    }
}