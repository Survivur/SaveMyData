using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Counter
{
    [SerializeField, ReadOnly]
    private int count;
    public int Count
    {
        get => count;
        set => this.count = math.clamp(value, 0, max);
    }

    [SerializeField, ReadOnly(true)]
    private int max;
    public int Max
    {
        get => max;
        set => max = value > 0 ? value : 1;
    }

    public Counter(int max = 0, int count = 0)
    {
        this.max = max;
        this.count = count <= 0 ? max : count;
    }

    public static implicit operator int(Counter c) => c.count;

    public void Reset() => count = max;

    /// <summary>
    /// Decreases the counter value by the given offset and returns true only when the value reaches 0.
    /// </summary>
    /// <param name="Offset">Optional offset to subtract from the counter value. Defaults to 1.</param>
    public bool Counting(int Offset = 1)
    {
        if (count > 0) count = math.max(0, count - Offset);
        return count == 0;
    }
}