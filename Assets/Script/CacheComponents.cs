using System;
using System.Collections.Generic;
using UnityEngine;

public static class CacheComponentsExtensions
{
     private static Dictionary<GameObject, CacheComponents> _instances = new();

    public static CacheComponents GetCacheComponent(this Transform tr)
    {
        return GetCacheComponent(tr.gameObject);
    }
    public static CacheComponents GetCacheComponent(this GameObject go)
    {
        if (_instances.TryGetValue(go, out var cache) && cache != null)
            return cache;

        if (!go.TryGetComponent(out cache))
            cache = go.AddComponent<CacheComponents>();

        _instances[go] = cache;
        return cache;
    }
    
    public static T GetComponentCached<T>(this Transform tr) where T : Component
    {
        return tr.GetCacheComponent().GetCached<T>();
    }

    public static T GetComponentCached<T>(this GameObject go) where T : Component
    {
        return go.GetCacheComponent().GetCached<T>();
    }

    public static void RemoveCacheComponents(this GameObject go)
    {
        _instances.Remove(go); // cleanup
    }
}

public class CacheComponents : MonoBehaviour
{
    private Dictionary<Type, Component> _cache = new();
    public T GetCached<T>() where T : Component
    {
        if (_cache.TryGetValue(typeof(T), out var cached) && cached != null)
            return (T)cached;

        T component = GetComponent<T>();
        _cache[typeof(T)] = component;
        return component;
    }
    private void OnDestroy()
    {
        gameObject.RemoveCacheComponents(); // cleanup
    }
}