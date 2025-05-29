#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ShowPropertyInInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var targetObject = target as object;
        if (targetObject == null) return;

        var type = targetObject.GetType();
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (var prop in type.GetProperties(flags))
        {
            if (Attribute.IsDefined(prop, typeof(ShowPropertyInInspectorAttribute)))
            {
                object value = prop.GetValue(targetObject);
                EditorGUILayout.LabelField(prop.Name, "p_" +value?.ToString());
            }
        }
    }
}
#endif
