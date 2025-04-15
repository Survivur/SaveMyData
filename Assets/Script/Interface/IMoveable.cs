using UnityEngine;

public interface IMoveable
{
    float Speed { get; }
    Vector2 Velocity { get; }
}

public static class IMoveableExtention
{
    public static Vector2 VelocityDefault<T>(this IMoveable moveable, T component) where T : Component
    {
        switch (component)
        {
            case Rigidbody2D rigidbody2D:
                if (rigidbody2D == null) return Vector2.positiveInfinity;
                return rigidbody2D.linearVelocity;
            case Transform transform:
                if (transform == null) return Vector2.positiveInfinity;
                return Vector2.zero;
            default:
                Debug.LogWarning($"Unsupported component type: {component.GetType().Name}");
                return default;
        }
    }

//     public static void UpdateVelocityDefault<T>(this IMoveable moveable,ref T component, float deltaTime = float.PositiveInfinity) where T : Component
//     {
//         switch (component)
//         {
//             case Rigidbody2D rigidbody2D:
//                 // Check if the rigidbody2D is null and return if it is
//                 if (rigidbody2D == null) return;

//                 // Set the linear velocity of the rigidbody2D to the velocity of the moveable
//                 rigidbody2D.linearVelocity = moveable.Velocity;
//                 break;
//             case Transform transform:
//                 if (transform == null) return;
//                 transform.position += (Vector3)moveable.Velocity * deltaTime.ChangeIfTrue(deltaTime == float.PositiveInfinity, Time.fixedDeltaTime);
//                 break;
//             default:
//                 Debug.LogWarning($"Unsupported component type: {component.GetType().Name}");
//                 break;
//         }
//     }
}