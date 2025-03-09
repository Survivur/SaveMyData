using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public List<string> TargetTags { get; }
    public float Damage { get; }
}
