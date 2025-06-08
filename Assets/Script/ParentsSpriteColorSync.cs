using UnityEngine;

public class ParentsSpriteColorSync : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        SpriteRenderer parentSpriteRenderer = GetComponent<SpriteRenderer>();
        if (parentSpriteRenderer != null) 
            gameObject.GetComponentCached<SpriteRenderer>().color =
                transform.parent.GetComponentCached<SpriteRenderer>().color;
    }
}
