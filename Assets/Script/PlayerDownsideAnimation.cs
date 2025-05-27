using UnityEngine;

public class PlayerDownsideAnimation : MonoBehaviour
{
    [Header("Sprite Controller")]
    [ReadOnly(true)] public SpriteController UpsideSpriteController;

    [Header("Info")]
    [ReadOnly, SerializeField] private Vector3 StartLocalPosition = Vector3.zero;

    private bool prevSeeRight = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (UpsideSpriteController == null) UpsideSpriteController = transform.parent.Find("Upside").GetComponent<SpriteController>();
        if (StartLocalPosition == Vector3.zero) StartLocalPosition = transform.localPosition;
        prevSeeRight = UpsideSpriteController.seeRight;
    }

    // Update is called once per frame
    void Update()
    {
        if (UpsideSpriteController.seeRight != prevSeeRight)
        {
            transform.localPosition =  new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            prevSeeRight = UpsideSpriteController.seeRight;
        }
    }
}
