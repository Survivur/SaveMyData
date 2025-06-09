using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] protected Counter counter = new Counter(1);
    [SerializeField] protected float startSpeed = 2f;
    [SerializeField] protected float minSpeed = 1f;
    [SerializeField] protected float coolDown = 2f;
    [SerializeField] protected float delay = 0.2f;

    [Header("Information")]
    [SerializeField, ReadOnly] protected bool goRight = false;
    [SerializeField, ReadOnly] protected bool dashFlag = false;
    [SerializeField, ReadOnly] protected bool isDashing = false;
    [SerializeField, ReadOnly] protected float velocityX = 1f;
    [SerializeField, ReadOnly] protected Coroutine resetCoroutine;

    [Header("Object & Components", order = 0)]
    [SerializeField, ReadOnly(true)] private SpriteRenderer DownsideChildSprite = null;
    [SerializeField, ReadOnly(true)] private PlayerMove playerMove = null;
    [SerializeField, ReadOnly(true)] private PlayerGhost playerGhost = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CodeExtensions.SetIfUnityNull(ref playerMove, GetComponent<PlayerMove>());
        CodeExtensions.SetIfUnityNull(ref playerGhost, GetComponent<PlayerGhost>());

        CodeExtensions.SetIfUnityNull(ref DownsideChildSprite, GameObjectRegistry.GetOrRegister(ObjectPath.Player_Downside, gameObject).GetComponentCached<SpriteRenderer>());

        Reset();
    }

    /// <summary>
    /// ?��
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>??? ?????</returns>
    public bool CalcurateVelocity(ref Vector2 velocity)
    {
        if (dashFlag && !isDashing && counter > 0)
        {
            velocityX = startSpeed;
            goRight = DownsideChildSprite.flipX;
            isDashing = true;
            dashFlag = false;
            Counting();
            Invoke(nameof(DashAble), delay);
        }

        if (velocityX > minSpeed)
        {
            velocity.x = goRight.BoolToSign() * velocityX * playerMove.Speed;
            velocityX -= Time.fixedDeltaTime * Mathf.Abs(startSpeed - 1f) / delay;

            playerGhost.SpawnGhost();
        }
        else
        {
            velocityX = minSpeed;
        }
        return velocity.x != 0;
    }

    public void Ready()
    {
        if (!isDashing && !dashFlag && counter > 0)
        {
            dashFlag = true;
        }
    }

    public void Reset()
    {
        counter.Reset();
    }

    private void Counting()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        resetCoroutine = StartCoroutine(counter.CountResetCoroutine(coolDown));
        counter.Counting();
    }
    
    public void DashAble()
    {
        isDashing = false;
    }    
}
