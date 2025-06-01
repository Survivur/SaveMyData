using System.Collections.Generic;
using UnityEngine;

public class GameObjectResource : Singleton<GameObjectResource>
{
    [SerializeField, ReadOnly(true)] public GameObject GhostManager;
    [SerializeField, ReadOnly(true)] public GameObject Canvas;
    [SerializeField, ReadOnly] public List<GameObject> CameraFocusObjects;

    protected override void Awake()
    {
        base.Awake();
        if (GhostManager == null) GhostManager = GameObject.Find("GhostManager");
        if (Canvas == null) Canvas = GameObject.Find("Canvas");
        if (GhostManager == null)
        {
            Debug.LogWarning("GhostManager not found");
            GhostManager = new GameObject("GhostManager");
        }
    }
}
