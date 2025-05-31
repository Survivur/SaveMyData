using UnityEngine;

public class ParriedHands : MonoBehaviour
{
    [SerializeField] private float MaxAlpha = 0.5f;
    [SerializeField, ReadOnly] private SpriteRenderer spriteRenderer;

    [SerializeField] private float totalLifetime = 1f;
    [SerializeField] private float fadeTime = 0.15f;

    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlpha(0f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer <= fadeTime)
        {
            // 증가
            float alpha = Mathf.Lerp(0f, MaxAlpha, timer / fadeTime);
            SetAlpha(alpha);
        }
        else if (timer <= totalLifetime - fadeTime)
        {
            // 유지
            SetAlpha(MaxAlpha);
        }
        else if (timer <= totalLifetime)
        {
            // 감소
            float t = (timer - (totalLifetime - fadeTime)) / fadeTime;
            float alpha = Mathf.Lerp(MaxAlpha, 0f, t);
            SetAlpha(alpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}