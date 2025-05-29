// ShowPropertyInInspectorDrawer.cs
using UnityEditor;
using UnityEngine;

// example
// [ShowPropertyInInspector]
// public int DoubleValue => value * 2;
//[CustomPropertyDrawer(typeof(ShowPropertyInInspectorAttribute))]
public class ShowPropertyInInspectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position, label.text, "Use with CustomEditor only");
    }
}
