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

    /// <summary>
    /// Calculates the centroid of all the objects that are in the focus of the camera.
    /// </summary>
    /// <returns>The centroid of all the objects in the focus of the camera.</returns>
    Vector2 CalculateCentroid()
    {
        // Get the number of objects that are in the focus of the camera
        int maxCount = GameObjectResource.Instance.CameraFocusObjects.Count;

        // If there are no objects in the focus, return the position of the camera
        if (maxCount == 0)
        {
            return transform.position;
        }

        // Calculate the sum of all the x and y positions of the objects in the focus
        float sumX = 0f;
        float sumY = 0f;

        foreach (var gObject in GameObjectResource.Instance.CameraFocusObjects)
        {
            // Get the position of the current object in the focus
            Vector3 pos = gObject.transform.position;

            // Add the x and y positions to the sum
            sumX += pos.x;
            sumY += pos.y;
        }

        // Calculate the centroid by dividing the sum of all the positions by the number of objects
        return new Vector2(sumX / maxCount, sumY / maxCount);
    }
}
