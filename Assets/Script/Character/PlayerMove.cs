using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour, IMoveable
{
    public virtual Vector2 Velocity => this.VelocityDefault(rigidbody2D);

    [Header("Info", order = 0)]
    [field: SerializeField] public float Speed { get; protected set; } = 10f;
    [SerializeField, ReadOnly] protected float HorizontalInput = 0f;

    [Header("Components", order = 0)]
    [SerializeField, ReadOnly(true)] protected Rigidbody2D rigidbody2D;
    [SerializeField, ReadOnly] protected PhotonView photonView;
    
    [SerializeField, ReadOnly(true)] private Player player = null;
    [SerializeField, ReadOnly(true)] private PlayerDash playerDash = null;
    [SerializeField, ReadOnly(true)] private PlayerJump playerJump = null;
    [SerializeField, ReadOnly(true)] private PlayerGhost playerGhost = null;

    [Header("Children", order = 0)]
    [SerializeField, ReadOnly(true)] private SpriteRenderer DownsideChildSprite = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CodeExtensions.SetIfUnityNull(ref rigidbody2D, GetComponent<Rigidbody2D>());
        CodeExtensions.SetIfNull(ref photonView, GetComponent<PhotonView>());

        CodeExtensions.SetIfUnityNull(ref player, GetComponent<Player>());
        CodeExtensions.SetIfUnityNull(ref playerDash, GetComponent<PlayerDash>());
        CodeExtensions.SetIfUnityNull(ref playerJump, GetComponent<PlayerJump>());
        CodeExtensions.SetIfUnityNull(ref playerGhost, GetComponent<PlayerGhost>());

        CodeExtensions.SetIfUnityNull(ref DownsideChildSprite, player.DownsideChild.GetComponent<SpriteRenderer>());
    }

    protected void FixedUpdate()
    {
        if (photonView.IsMine)
            photonView.RPC(nameof(RPC_HorizionInput), RpcTarget.All, Input.GetAxisRaw("Horizontal"));
        rigidbody2D.linearVelocity = UpdateVelocity(rigidbody2D.linearVelocity);
    }

    /// <summary>
    /// FixedUpdate���� ����ȴ�.
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns></returns>
    protected Vector2 UpdateVelocity(Vector2 velocity)
    {
        HorizontalMovement(ref velocity);
        playerDash.CalcurateVelocity(ref velocity);
        playerJump.CalcurateVelocity(ref velocity);
        return velocity;
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
            DownsideChildSprite.flipX = HorizontalInput > 0f;
        }

        velocity.x = HorizontalInput * Speed;

        return velocity.x != 0;
    }
    
    [PunRPC]
    public void RPC_HorizionInput(float horizontalInput)
    {
        HorizontalInput = horizontalInput;        
    }
}
