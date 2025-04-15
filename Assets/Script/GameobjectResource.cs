using System.Collections.Generic;
using UnityEngine;

public class GameObjectResource : Singleton<GameObjectResource>
{
    [SerializeField, ReadOnly(true)] public GameObject GhostManager;
    [SerializeField, ReadOnly] public List<GameObject> CameraFocusObjects;

    private void Start()
    {
        GhostManager = GameObject.Find("GhostManager");
        if (GhostManager == null)
        {
            Debug.LogWarning("GhostManager not found");
            GhostManager = new GameObject("GhostManager");
        }
    }
}
