using UnityEngine;

public class ObjectFollowUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private GameObject targetObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(targetObject.transform.position + offset);
        }
    }
}
