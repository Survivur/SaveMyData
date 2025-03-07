using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isRight = true;
    public float damage = 10f;
    public float speed = 10f;
    public List<string> targetTags = new List<string>();

    private Rigidbody characterRigidbody;

    void Start(){
        characterRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3(speed * (isRight ? 1f : -1f), 0, 0);
        characterRigidbody.linearVelocity = velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!targetTags.Contains(other.tag)) return;
        
        Hitable hitable = other.GetComponent<Hitable>();
        if (hitable != null)
        {
            hitable.OnHit();
        }
        Destroy(gameObject);
    }
}
