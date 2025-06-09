using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameObjectRegistry : Singleton<GameObjectRegistry>
{
private static readonly Dictionary<int, GameObject> _cache = new();

    public static GameObject GetOrRegister(string path, GameObject parent = null)
    {
        if (parent != null && parent.IsUnityNull()) return null;
        if (string.IsNullOrEmpty(path)) return parent;
        if (path[0] == '/') path = path.Substring(1);

        if (parent == null)
        {
            string[] parts = path.Split('/');
            if (parts.Length == 0)
            {
                Debug.LogWarning("[Registry] Invalid path: empty");
                return null;
            }

            parent = GameObject.Find(parts[0]);
            if (parent == null)
            {
                Debug.LogWarning($"[Registry] Root GameObject '{parts[0]}' not found.");
                return null;
            }

            path = string.Join("/", parts, 1, parts.Length - 1);
        }

        Transform child = parent.transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning($"[Registry] Child not found: {path} under {GetFullPath(parent.transform)}");
            return null;
        }

        GameObject result = child.gameObject;
        RegisterFullHierarchy(result.transform);
        return result;
    }

    private static void RegisterFullHierarchy(Transform tr)
    {
        while (tr != null)
        {
            int id = tr.gameObject.GetInstanceID();
            if (!_cache.ContainsKey(id) || _cache[id] == null)
                _cache[id] = tr.gameObject;

            tr = tr.parent;
        }
    }

    public static void Register(GameObject go)
    {
        if (go != null)
            _cache[go.GetInstanceID()] = go;
    }

    public static void Clear()
    {
        _cache.Clear();
    }


    /// <summary>
    /// 현재 트랜스폼에서 루트까지 전체 경로 문자열 생성
    /// </summary>
    private static string GetFullPath(Transform tr)
    {
        List<string> names = new();
        while (tr != null)
        {
            names.Insert(0, tr.name);
            tr = tr.parent;
        }
        return string.Join("/", names);
    }
}
