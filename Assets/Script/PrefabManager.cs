using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [SerializeField, ReadOnly] private GameObject ghostPrefab;
    [SerializeField, ReadOnly] private GameObject parriedHandPrefab;

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
}