using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct DashData
{
    [Header("Options")]
    [SerializeField, ReadOnly(true)] public float startSpeed;
    [SerializeField, ReadOnly(true)] public float minSpeed;
    [SerializeField, ReadOnly(true)] public int maxCount;
    [SerializeField, ReadOnly(true)] public float coolDown;
    [SerializeField, ReadOnly(true)] public float delay;

    [Header("Information")]
    [SerializeField, ReadOnly] public bool goRight;
    [SerializeField, ReadOnly] public bool isKeyDown;
    [SerializeField, ReadOnly] public float velocity;
    [SerializeField, ReadOnly] public int count;
}

[System.Serializable]
public struct JumpInfo
{
    [SerializeField] public float speed;
    [SerializeField, ReadOnly(true)] public int countMax;
    [SerializeField, ReadOnly(true)] public int count;
    [SerializeField, ReadOnly(true)] public bool input;
    [SerializeField, ReadOnly(true)] public bool isJumping;
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

[System.Serializable]
public struct PlayerNameUI
{
    [SerializeField, ReadOnly(true)] public string playerName;
    [SerializeField, ReadOnly(true)] public string nameTextName;
}

public class Player : Character
{
    private PhotonView photonView; // 포톤 서버 관련 추가
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
        velocity = 1f,
        count = 0,
    };


    [SerializeField, ReadOnly(true)] private string playerName = "John Wick";
    [SerializeField, ReadOnly(true)] private string nameText_name = "PlayerName";

    [SerializeField, ReadOnly(true)] private float moveSpeed = 1.0f;




    [Header("Options", order = 0)]

    [SerializeField] private float JumpSpeed = 7f;
    [SerializeField, ReadOnly(true)] private int JumpCountMax = 1;
    [SerializeField, ReadOnly] private int JumpCount = 1;
    [SerializeField, ReadOnly] private bool JumpInput = false;
    [SerializeField, ReadOnly] private bool isJumping = false;
    [Header("ghost", order = 1)]
    [SerializeField, ReadOnly(true)] private float ghostDuration = 0.3f;

    [Header("Shoot", order = 1)]
    [SerializeField, ReadOnly(true)] private int bulletCountMax = 10;
    [SerializeField, ReadOnly(true)] private int bulletCount = 0;
    [SerializeField] private float reloadSpeed = 1.5f;
    [SerializeField, ReadOnly(true)] private float bulletSpeed = 10f;
    [SerializeField, ReadOnly(true)] private float bulletDelay = 0.3f;
    [SerializeField, ReadOnly(true)] private float ShootGap = 1f;

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

        if (UpsideChild == null) UpsideChild = transform.Find("Upside").gameObject;
        if (UpsideChildSprite == null) UpsideChildSprite = UpsideChild.GetComponent<SpriteRenderer>();
        if (DownsideChild == null) DownsideChild = transform.Find("Downside").gameObject;
        if (ArmChild == null) ArmChild = transform.Find("Arm").gameObject;

        playerName = (PhotonNetwork.NickName != "") ? PhotonNetwork.NickName : playerName;
        nameText.text = playerName;

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
            dash.isKeyDown = true;
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
        if (dash.isKeyDown && dash.count > 0)
        {
            DashCounting();
            dash.isKeyDown = false;
        }

        if (!JumpInput)
        {
            isJumping = false;
        }
    }

    protected override void OnDestroy()
    {

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
        velocity = CalcurateDashVelocity();
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
        Vector2 CalcurateDashVelocity()
        {
            bool result = false;
            return CalcurateDashVelocity(out result);
        }

        Vector2 CalcurateDashVelocity(out bool result)
        {
            Vector2 velocity = Vector2.zero;
            if (dash.isKeyDown && dash.count > 0)
            {
                dash.velocity = dash.startSpeed;
                dash.goRight = seeRight;
            }

            if (dash.velocity > dash.minSpeed)
            {
                velocity.x = dash.goRight.BoolToSign() * Speed * dash.velocity;
                dash.velocity -= Time.fixedDeltaTime * Mathf.Abs(dash.startSpeed - 1f) / dash.delay;

                if (!IsInvoking(nameof(SpawnGhostCoroutine)))
                {
                    StartCoroutine(SpawnGhostCoroutine(ghostDuration));
                }
            }
            else
            {
                dash.velocity = dash.minSpeed;
            }
            result = velocity.x != 0;
            return velocity;
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
            Vector2 dir = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;

            //네트워크를 통해 총알 생성
            GameObject bulletObj = CreateBullet(
                ObjectPath.Bullet,
                transform.position + (Vector3)dir,
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
            b.dir = dir;
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