using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameObjectRegistry : Singleton<GameObjectRegistry>
{
    private static readonly Dictionary<string, GameObject> _cache = new();

    void Start()
    {
        
    }

    // /// <summary>
    // /// 경로 기반으로 GameObject를 찾아 캐싱 (예: "UI/Panel/Button")
    // /// </summary>
    // public GameObject GetOrRegister(string path)
    // {
    //     if (string.IsNullOrEmpty(path)) return null;

    //     if (path[0] == '/') path = path.Substring(1);
    //     string[] parts = path.Split('/');
    //     if (parts.Length == 0) return null;

    //     GameObject root = GameObject.Find(parts[0]);
    //     if (root == null)
    //     {
    //         Debug.LogWarning($"[Registry] Root GameObject '{parts[0]}' not found.");
    //         return null;
    //     }

    //     // 상대 경로로 위임
    //     if (parts.Length == 1)
    //         return GetOrRegister(string.Empty, root); // 자기 자신

    //     string relativePath = string.Join("/", parts, 1, parts.Length - 1);
    //     return GetOrRegister(relativePath, root);
    // }

    /// <summary>
    /// 부모 GameObject 기준 상대 경로로 자식 탐색 및 등록
    /// </summary>
    public static GameObject GetOrRegister(string path, GameObject parent = null)
    {
        if (parent.IsUnityNull()) return null;
        if (string.IsNullOrEmpty(path)) return parent;
        if (path[0] == '/') path = path.Substring(1);

        // 경로로부터 루트 오브젝트 이름 추출 → parent 지정
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

            // 상대 경로에서 루트 이름 제거
            path = string.Join("/", parts, 1, parts.Length - 1);
        }

        string basePath = GetFullPath(parent.transform);
        string fullPath = $"{basePath}/{path}";

        if (_cache.TryGetValue(fullPath, out var cached) && cached != null)
            return cached;

        Transform child = parent.transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning($"[Registry] Child not found: {path} under {basePath}");
            return null;
        }

        GameObject result = child.gameObject;
        RegisterFullHierarchy(result.transform); // 모든 상위 포함 등록
        return result;
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

    /// <summary>
    /// 주어진 Transform에서 루트까지 모든 GameObject를 캐시에 등록
    /// </summary>
    private static void RegisterFullHierarchy(Transform tr)
    {
        while (tr != null)
        {
            string path = GetFullPath(tr);
            if (!_cache.ContainsKey(path) || _cache[path] == null)
                _cache[path] = tr.gameObject;

            tr = tr.parent;
        }
    }


    /// <summary>
    /// 강제로 캐시 갱신
    /// </summary>
    public static void Register(string path, GameObject go)
    {
        if (!string.IsNullOrEmpty(path) && go != null)
        {
            _cache[path] = go;
        }
    }

    /// <summary>
    /// 캐시 초기화
    /// </summary>
    public static void Clear()
    {
        _cache.Clear();
    }
}
