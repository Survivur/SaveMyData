using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ReadOnlyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //SerializedProperty property = serializedObject.GetIterator();

        SerializedProperty iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                if (iterator.propertyType == SerializedPropertyType.Generic)
                {
                    DrawPropertyAndChildrenWithFoldout(iterator);
                }
                else
                {
                    DrawWithReadOnlyCheck(iterator);
                }
            }
            while (iterator.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();

        // // 기본 인스펙터 UI 유지
        // //DrawDefaultInspector();

        // serializedObject.Update();

        // // 모든 필드 순회
        // SerializedProperty property = serializedObject.GetIterator();
        // // 스크립트 파일이면
        // if (property.NextVisible(true))
        // {
        //     do
        //     {
        //         using (new EditorGUI.DisabledScope(property.name[0] == 'm'))
        //         {
        //             //Debug.Log($"{property.name} value is {property.GetType()}");
        //             if (property is SerializedProperty)
        //             {
        //                 DrawPropertyWithChildren(property);
        //             }
        //         }

        //     }
        //     while (property.NextVisible(false));
        // }

        // serializedObject.ApplyModifiedProperties();
    }

    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    private void DrawPropertyAndChildrenWithFoldout(SerializedProperty property)
    {
        // 고유 키를 위한 경로 사용
        string key = property.propertyPath;
        if (!foldoutStates.ContainsKey(key))
            foldoutStates[key] = false;

        foldoutStates[key] = EditorGUILayout.Foldout(foldoutStates[key], property.displayName, true);

        if (foldoutStates[key])
        {
            EditorGUI.indentLevel++;

            SerializedProperty child = property.Copy();
            SerializedProperty end = child.GetEndProperty();

            bool enterChildren = true;

            while (child.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(child, end))
                    break;

                if (child.propertyType == SerializedPropertyType.Generic)
                    DrawPropertyAndChildrenWithFoldout(child); // 재귀
                else
                    DrawWithReadOnlyCheck(child);

                enterChildren = false;
            }

            EditorGUI.indentLevel--;
        }
    }


    public static void DrawWithReadOnlyCheck(SerializedProperty property)
    {
        object target = property.serializedObject.targetObject;
        FieldInfo fieldInfo = GetFieldInfoFromPropertyPath(target.GetType(), property.propertyPath);

        var readOnlyAttr = fieldInfo?.GetCustomAttribute<ReadOnlyAttribute>();

        using (new EditorGUI.DisabledScope(readOnlyAttr != null && (Application.isPlaying || !readOnlyAttr.runtimeOnly)))
        {
            EditorGUILayout.PropertyField(property, false);
        }
    }

    private static FieldInfo GetFieldInfoFromPropertyPath(Type rootType, string path)
    {
        string[] elements = path.Split('.');
        Type currentType = rootType;
        FieldInfo field = null;

        for (int i = 0; i < elements.Length; i++)
        {
            string element = elements[i];

            // 배열이나 리스트 처리 (예: myList.Array.data[0].value)
            if (element == "Array" && i + 2 < elements.Length && elements[i + 1].StartsWith("data["))
            {
                i += 2; // Skip "Array" and "data[index]"
                continue;
            }

            field = currentType.GetField(element,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null) return null;

            currentType = field.FieldType;
        }

        return field;
    }
}