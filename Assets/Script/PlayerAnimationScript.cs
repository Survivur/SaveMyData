using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [Header("Components")]
    [ReadOnly, SerializeField] private Animator animatior;
    [ReadOnly, SerializeField] private SpriteController spriteController;

    
    static int prevSeeAngle = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animatior = GetComponent<Animator>();
        spriteController = GetComponent<SpriteController>();

        spriteController.rotationType = RotationType.rotation180;
    }

    // Update is called once per frame
    void Update()
    {
        float seeAngle = spriteController.MouseAimAngle / 22.5f;

        animatior.SetBool("needChange", (int)seeAngle != prevSeeAngle);
        animatior.SetInteger("SeeType", (int)seeAngle);

        prevSeeAngle = (int)seeAngle;
    }
}
