using UnityEngine;

public class PlayerUpsideAnimation : MonoBehaviour
{
    [Header("Components")]
    [ReadOnly, SerializeField] private Animator animator;
    [ReadOnly, SerializeField] private SpriteController spriteController;

    static int prevSeeAngle = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteController = GetComponent<SpriteController>();

        spriteController.rotationType = RotationType.rotation180;
    }

    // Update is called once per frame
    void Update()
    {
        float seeAngle = spriteController.MouseAimAngle / 22.5f;

        animator.SetBool("needChange", (int)seeAngle != prevSeeAngle);
        animator.SetInteger("SeeType", (int)seeAngle);

        prevSeeAngle = (int)seeAngle;
    }
}
