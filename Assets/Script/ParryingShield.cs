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
        SpawnHands(-direction);
        bullet.dir = ParentsCharacter.AimPosition;
        ParentsCharacter.Shoot(bullet);
    }


    void SpawnHands(Vector3 dir)
    {
        GameObject parriedHand = Instantiate(PrefabManager.Instance.ParriedHand,
            transform.position - new Vector3(0, 0, 1f),
            Quaternion.Euler(0, 0, 180+ Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg + rotateGap),
            transform);

        //Debug.Log(ghost.transform.rotation.z);
        //ghost.transform.localScale = transform.localScale;
        SpriteRenderer spriteRenderer = parriedHand.GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = GetComponent<SpriteRenderer>().flipX;
        
    }

    void OnEnable()
    {
        //ParentsCharacter.GetComponent<Collider2D>().enabled = false;
    }

    private void OnDisable() {
        //ParentsCharacter.GetComponent<Collider2D>().enabled = true;
    }
}
