using UnityEngine;

public interface IMoveable
{
    float Speed { get; }
    Vector2 Velocity { get; }

    public static Vector2 VelocityDefault(Rigidbody2D rigidbody2D, IMoveable moveable)
    {
        if (rigidbody2D == null) return Vector2.positiveInfinity;

        return rigidbody2D.linearVelocity;
    }

    public static Vector2 VelocityDefault(Transform transform, IMoveable moveable)
    {
        if (transform == null) return Vector2.positiveInfinity;

        return Vector2.zero;
    }

    public static void UpdateVelocity(Rigidbody2D rigidbody2D, IMoveable moveable)
    {
        if (rigidbody2D == null) return;

        rigidbody2D.linearVelocity = moveable.Velocity;
    }

    public static void UpdateVelocity(Transform transform, IMoveable moveable, float deltaTime = 0f)
    {
        if (transform == null) return;

        transform.position += (Vector3)moveable.Velocity * deltaTime.ChangeIfTrue(deltaTime == 0f, Time.fixedDeltaTime);
    }
}
