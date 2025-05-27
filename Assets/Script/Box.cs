using Microsoft.Unity.VisualStudio.Editor;
using Mono.Cecil;
using UnityEngine;

public class Box : MonoBehaviour, IHittable
{
    [Header("Option")]
    // Hittable
    [ReadOnly(true)] public float MaxHealth = 10f;
    public float Health { get; private set; }

    [Header("resource")]
    [ReadOnly(true)] public Sprite[] boxImage = null;
    [ReadOnly(true)] public SpriteRenderer spriteRenderer = null;

    public void Start()
    {
        if (boxImage.Length == 0) boxImage = Resources.LoadAll<Sprite>("Sprite/box");
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        Health = MaxHealth;
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        Health -= damage;

        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
        else if (Health <= MaxHealth / 4)
        {
            spriteRenderer.sprite = boxImage[2];
        }
        else if (Health <= MaxHealth / 2)
        {
            spriteRenderer.sprite = boxImage[1];
        }
    }
}
