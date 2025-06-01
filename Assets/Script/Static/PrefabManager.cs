using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [SerializeField, ReadOnly] private GameObject ghostPrefab;
    public GameObject Ghost
    {
        get
        {
            if (ghostPrefab == null)
            {
                ghostPrefab = Resources.Load<GameObject>("Prefabs/Ghost");

            }
            return ghostPrefab;
        }
    }

    [SerializeField, ReadOnly] private GameObject parriedHandPrefab;
    public GameObject ParriedHand
    {
        get
        {
            if (parriedHandPrefab == null)
            {
                parriedHandPrefab = Resources.Load<GameObject>("Prefabs/ParriedHand");

            }
            return parriedHandPrefab;
        }
    }

    [SerializeField, ReadOnly] private GameObject healthBar;
    public GameObject HealthBar
    {
        get
        {
            if (healthBar == null)
            {
                healthBar = Resources.Load<GameObject>(ObjectPath.HpBar);
            }
            return healthBar;
        }
    }

    [SerializeField, ReadOnly] private GameObject nameUI;
    public GameObject NameUI
    {
        get
        {
            if (nameUI == null)
            {
                nameUI = Resources.Load<GameObject>(ObjectPath.HpBar);
            }
            return nameUI;
        }
    }
}