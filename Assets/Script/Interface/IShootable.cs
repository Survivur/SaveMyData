using UnityEngine;

public interface IShootable
{
    float Speed { get; }
    public void Shoot(float? damage, Vector2? dir, bool isBlockedByBlock);
    public void Shoot(Bullet bullet);
}
