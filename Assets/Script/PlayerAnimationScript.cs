using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    [ReadOnly] public float Angle = 0;
    [ReadOnly] public Animator animatior;
    [ReadOnly] public SpriteRenderer spriteRenderer;

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

        float seeAngle = 0;

        if (Angle < -90f || Angle > 90f)
        {
            seeAngle = 180f - ((Angle + 180f) % 180f);
        }
        else
        {
            seeAngle = (Angle + 90f / 8);
        }
        animatior.SetInteger("SeeType", (int)seeAngle);
    }

    private float UpdateAngle()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
