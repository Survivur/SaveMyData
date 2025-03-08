using UnityEngine;

public interface IMoveable
{
    float Speed { get; }
    Vector2 Velocity { get; }

    public static Vector2 VelocityDefault(Rigidbody2D rigidbody2D, IMoveable moveable)
    {
        if (rigidbody2D == null) return Vector2.zero;

        return rigidbody2D.linearVelocity;
    }

    public static void UpdateVelocity(Rigidbody2D rigidbody2D, IMoveable moveable)
    {
        if (rigidbody2D == null) return;

        rigidbody2D.linearVelocity = moveable.Velocity;
    }
}
