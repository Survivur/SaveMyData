using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
 
public class CharacterMove : MonoBehaviour
{
    public float speed = 5f;
    public float JumpSpeed = 3f;
    private Rigidbody characterRigidbody;
    private GameObject bullet;

    void Start(){
        characterRigidbody = GetComponent<Rigidbody>();
    }

    void Awake()
    {
        bullet = Resources.Load<GameObject>("Prefabs/Bullet");
        Debug.Log(bullet);
    }

    void Update(){ 
        //float inputX = ;
        //float inputZ = Input.GetAxis("Vertical");
        if (Input.GetKeyDown("j"))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        velocity *= speed;
        if (Input.GetAxis("Jump") > 0)
        {
            velocity.y += JumpSpeed;
        }
        else
        {
            velocity.y += characterRigidbody.linearVelocity.y;
        }
        characterRigidbody.linearVelocity = velocity;
    }

    void Shoot()
    {
        Bullet b = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
        b.targetTags.Add("Enemy");
    }
}