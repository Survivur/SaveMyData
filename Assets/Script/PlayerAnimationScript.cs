using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [ReadOnly] public float Angle = 0;
    [ReadOnly] public Animator animatior;
    [ReadOnly] public SpriteRenderer spriteRenderer;
    [ReadOnly, SerializeField] private float seeAngle = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animatior = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Angle = UpdateAngle();
        spriteRenderer.flipX = !(Angle < -90f || Angle > 90f);

        if (Angle < -90f || Angle > 90f)
        {
            seeAngle = 180f - ((Angle + 270f) % 180f);
        }
        else
        {
            seeAngle = Angle + 90f;
        }

        seeAngle = seeAngle / 22.5f;

        animatior.SetInteger("SeeType", (int)seeAngle);
    }

    private float UpdateAngle()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
