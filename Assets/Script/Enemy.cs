using UnityEngine;

public class Enemy : MonoBehaviour, Hitable
{
    private float health = 100f;
    public float Health    
    {
        get => health;
        set => health = value;
    }
    public float damage = 10f;
    public float speed = 10f;

    private Rigidbody characterRigidbody;

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3(speed, 0, 0);
        characterRigidbody.linearVelocity = velocity;
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     Hitable hitable = other.GetComponent<Hitable>();
    //     if (hitable != null)
    //     {
    //         
    //         hitable.OnHit();
    //     }
    //     Destroy(gameObject);
    // }

    public void OnHit()
    {
        Health -= 10;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}