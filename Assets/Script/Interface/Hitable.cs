using UnityEngine;

public interface Hitable
{
    float Health { get; set; }
    void OnHit();
}