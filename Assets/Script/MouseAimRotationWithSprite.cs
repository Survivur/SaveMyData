using UnityEngine;

public class MouseAimRotationWithSprite : MonoBehaviour
{
    [Header("Components")]
    [ReadOnly, SerializeField] private SpriteController spriteController;

    [ReadOnly, SerializeField] private float seeAngle = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteController = GetComponent<SpriteController>();
        
        spriteController.rotationType = RotationType.rotation90;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, spriteController.MouseAimAngle);
    }
}
