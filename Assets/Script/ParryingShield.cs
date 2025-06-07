using UnityEngine;

public class ParryingShield : MonoBehaviour, IHittable
{
    [field: SerializeField, ReadOnly] public float Health { get; protected set; } = -1f;
    [SerializeField] private float rotateGap = -50f;

    [Header("Components")]
    [SerializeField, ReadOnly] Character ParentsCharacter;


    void Start()
    {
        ParentsCharacter = transform.parent.GetComponent<Character>();
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        // do nothing
    }

    public void TakeDamage(Bullet bullet, Vector2 direction)
    {
        SpawnHands(bullet.dir);
        bullet.dir = ParentsCharacter.AimDirection;
        ParentsCharacter.Shoot(bullet);
        Destroy(bullet);
    }


    void SpawnHands(Vector3 dir)
    {
        GameObject parriedHand = Instantiate(PrefabManager.Instance.ParriedHand,
            transform.position,
            Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotateGap),
            transform);


        //Debug.Log(parriedHand.transform.rotation.z);
        //ghost.transform.localScale = transform.localScale;
        SpriteRenderer spriteRenderer = parriedHand.GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = Random.Range(0, 2) == 1 ? true : false;
        //Debug.Log(spriteRenderer.flipX);
        
    }

    void OnEnable()
    {
        //ParentsCharacter.GetComponent<Collider2D>().enabled = false;
    }

    private void OnDisable() {
        //ParentsCharacter.GetComponent<Collider2D>().enabled = true;
    }
}
