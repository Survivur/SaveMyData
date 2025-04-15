using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [SerializeField] private GameObject ghostPrefab;
    
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
}