using System;
using UnityEngine;

public enum RotationType
{
    rotation90 = 0,
    rotation180 = 1
}

public class SpriteController : MonoBehaviour
{
    [Header("Options")]
    [ReadOnly(true)] public bool needFlip = true;
    [ReadOnly(true)] public bool isChangingPosition = true;
    [ReadOnly(true)] public RotationType rotationType = RotationType.rotation180;
    [ReadOnly(true)] public Vector3 FlipPostionGap = Vector3.zero;

    [Header("Components")]
    [ReadOnly, SerializeField] private SpriteRenderer spriteRenderer = null;
    [ReadOnly, SerializeField] private Player player = null;

    [Header("Info")]
    [ReadOnly] public float MouseAimAngle = 0;
    [ReadOnly, SerializeField] private float Angle = 0f;

    public bool seeRight => !(Angle < -90f || Angle > 90f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObjectRegistry.GetOrRegister("", transform.parent.gameObject).GetComponentCached<Player>();
        if (FlipPostionGap == Vector3.zero) FlipPostionGap.x = transform.localPosition.x * 2;
    }

    // Update is called once per frame
    void Update()
    {
        Angle = UpdateAngle();
        if (needFlip)
        {
            spriteRenderer.flipX = seeRight;
        }
        if (isChangingPosition)
        {
            transform.localPosition += FlipPostionGap * (spriteRenderer.flipX ? -1f : 0f);
        }

        switch (rotationType)
        {
            case RotationType.rotation90:
                if (Angle < -90f || Angle > 90f)
                {
                    MouseAimAngle = Angle + 180f;
                }
                else
                {
                    MouseAimAngle = Angle;
                }
                break;
            case RotationType.rotation180:
                if (Angle < -90f || Angle > 90f)
                {
                    MouseAimAngle = 180f - ((Angle + 270f) % 180f);
                }
                else
                {
                    MouseAimAngle = Angle + 90f;
                }   
                break;
            default:
                break;
        }
    }

    private float UpdateAngle()
    {
        Vector3 dir = (Vector3)player.AimDirection;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
