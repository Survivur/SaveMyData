using UnityEngine;

public class PlayerDownsideAnimation : MonoBehaviour
{
    [Header("Sprite Controller")]
    [ReadOnly(true)] public SpriteController UpsideSpriteController;

    [Header("Component")]
    [ReadOnly(true)] public Animator animator;
    [ReadOnly, SerializeField] PlayerMove playerMove;
    [ReadOnly, SerializeField] PlayerJump playerJump;

    [Header("Info")]
    [ReadOnly, SerializeField] private Vector3 StartLocalPosition = Vector3.zero;

    private bool prevSeeRight = false;
    private int prevCount = 0;

    public bool isSit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UpsideSpriteController == null) UpsideSpriteController = transform.parent.Find("Upside").GetComponent<SpriteController>();
        if (StartLocalPosition == Vector3.zero) StartLocalPosition = transform.localPosition;
        prevSeeRight = UpsideSpriteController.seeRight;
        animator = gameObject.GetComponentCached<Animator>();

        CodeExtensions.SetIfUnityNull(ref playerMove, transform.parent.GetComponentCached<PlayerMove>());
        CodeExtensions.SetIfUnityNull(ref playerJump, transform.parent.GetComponentCached<PlayerJump>());
    }

    // Update is called once per frame
    void Update()
    {
        if (UpsideSpriteController.seeRight != prevSeeRight)
        {
            transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            prevSeeRight = UpsideSpriteController.seeRight;
        }
        
        animator.SetBool("needChange", prevCount != playerJump.Counter.Count);
        animator.SetBool("isJump", playerMove.Velocity.y != 0);
        animator.SetBool("isSit", isSit);
        animator.SetBool("isWalk", playerMove.Velocity.x != 0);

        prevCount = playerJump.Counter.Count;
    }
}
