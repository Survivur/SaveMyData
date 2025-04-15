using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField, ReadOnly(true)] private float StartTime = 5.0f;
    public float CurrentTime { get; private set; }
    
    void Start()
    {
        CurrentTime = StartTime;
    }

    void Update()
    {
        if (CurrentTime <= 0.0f)
        {
            Destroy(gameObject);
        }
        else
        {
            CurrentTime -= Time.deltaTime;
        }
    }
}
