using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] protected Counter counter = new Counter();
    public Counter Counter => counter;
    [SerializeField] protected float speed = 15f;
    [SerializeField] protected float delay = 0.2f;

    [Header("Information")]
    [SerializeField, ReadOnly] protected bool isJumping = false;
    public bool IsJumping => isJumping;
    [SerializeField, ReadOnly] protected bool jumpFlag = false;

    public void Start()
    {
        Reset();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Ground) | collision.gameObject.CompareTag(Tags.Box))
        {
            if (collision.transform.position.y < transform.position.y)
                Reset();
        }
    }

    /// <summary>
    /// ?��
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>??? ?????</returns>
    public bool CalcurateVelocity(ref Vector2 velocity)
    {
        if (jumpFlag && !isJumping && counter > 0)
        {
            velocity.y = speed;
            isJumping = true;
            jumpFlag = false;
            Counting();
            Invoke(nameof(JumpAble), delay);
        }

        return velocity.y != 0;
    }

    public void Ready()
    {
        if (!isJumping && !jumpFlag && counter > 0)
        {
            jumpFlag = true;
        }
    }


    private void Counting()
    {
        counter.Counting();
    }

    public void Reset()
    {
        counter.Reset();
        JumpAble();
    }

    public void JumpAble()
    {
        isJumping = false;
    }     
    
    public bool JumpCountMax()
    {
        return counter.Count == counter.Max;
    }    
}
