using System;
using Unity.Mathematics;
using UnityEngine;

public class CameraMove : MonoBehaviour, IMoveable
{
    public float Speed { get; protected set; } = 5f;
    public virtual Vector2 Velocity => Speed * (CalculateCentroid() - (Vector2)transform.position);

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (transform == null) return;
        transform.position += (Vector3)Velocity * Time.fixedDeltaTime;
    }

    Vector2 CalculateCentroid()
    {
        int maxCount = GameObjectResource.Instance.CameraFocusObjects.Count;

        if (maxCount == 0)
            return transform.position;

        float sumX = 0f;
        float sumY = 0f;

        foreach (var gObject in GameObjectResource.Instance.CameraFocusObjects)
        {
            Vector3 pos = gObject.transform.position;
            sumX += pos.x;
            sumY += pos.y;
        }

        return new Vector2(sumX / maxCount, sumY / maxCount);
    }
}
