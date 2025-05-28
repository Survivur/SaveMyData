using System.Collections;
using UnityEngine;

public class SpriteShaker : MonoBehaviour
{
    [Header("option")]
    [ReadOnly(true), SerializeField] private Vector2 direction = new Vector2(0f, 0.1f);
    public Vector2 Direction => direction;
    [ReadOnly(true), SerializeField] private float roundTripDuration = 2f;
    public float RoundTripDuration => roundTripDuration;
    [ReadOnly(true), SerializeField] private int maxTime = 3;
    public int MaxTime => maxTime;

    [Header("info")]
    [ReadOnly(true)] private int nTime = 0;
    public int N_Time => nTime;
    [ReadOnly(true)] private bool isGoingUp = true;
    public bool IsGoingUp => isGoingUp;

    void Start()
    {
        nTime = maxTime;
        StartCoroutine(nameof(ShakeCoroutine));
    }

    IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            transform.localPosition += isGoingUp.BoolToSign() * (Vector3)direction;
            nTime--;
            if (nTime == 0)
            {
                isGoingUp = !isGoingUp;
                nTime = maxTime;
            }
            yield return new WaitForSeconds(roundTripDuration / (maxTime * 2));
        }
    }
}
