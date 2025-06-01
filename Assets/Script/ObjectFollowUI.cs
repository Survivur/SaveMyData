using UnityEngine;

public class ObjectFollowUI : MonoBehaviour
{
    [ReadOnly(true)] public Vector3 Offset = Vector3.zero;
    [ReadOnly(true)] public GameObject TargetObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TargetObject == null) TargetObject = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetObject != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(TargetObject.transform.position + Offset);
        }
    }
}
