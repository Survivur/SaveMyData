using System.Collections;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    [Header("Option", order = 1)]
    [SerializeField, ReadOnly(true)] private float duration = 0.3f;

    [Header("Components", order = 0)]
    [SerializeField, ReadOnly] protected SpriteRenderer spriteRenderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CodeExtensions.SetIfUnityNull(ref spriteRenderer, gameObject.GetComponentCached<SpriteRenderer>());
    }

    public void SpawnGhost()
    {
        if (!IsInvoking(nameof(SpawnGhostCoroutine)))
        {
            StartCoroutine(SpawnGhostCoroutine(duration));
        }
    }
    
    private void CreateGhost()
    {
        GameObject ghost = Instantiate(PrefabManager.Instance.Ghost,
            transform.position - new Vector3(0, 0, 1f),
            Quaternion.identity,
            GameObjectResource.Instance.GhostManager.transform);
        ghost.transform.localScale = transform.localScale;
        SpriteRenderer spriteRenderer = ghost.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.spriteRenderer.sprite;
        spriteRenderer.flipX = this.spriteRenderer.flipX;
        spriteRenderer.color = new Color(1, 1, 1, 0.2f);
    }

    public IEnumerator SpawnGhostCoroutine(float delay)
    {
        CreateGhost();
        yield return new WaitForSeconds(delay);
    }
}
