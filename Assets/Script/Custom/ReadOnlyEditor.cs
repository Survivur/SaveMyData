using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ReadOnlyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ �ν����� UI ����
        //DrawDefaultInspector();

        serializedObject.Update();

        // ��� �ʵ� ��ȸ
        SerializedProperty property = serializedObject.GetIterator();
        // ��ũ��Ʈ �����̸�
        if (property.NextVisible(true))
        {
            do
            {
                using (new EditorGUI.DisabledScope(property.name[0] == 'm'))
                {
                    //Debug.Log($"{property.name} value is {property.GetType()}");
                    if (property is SerializedProperty)
                    {
                        DrawPropertyWithChildren(property);
                    }
                }
                
            }
            while (property.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();

        // [ShowPropertyInInspector] �Ӽ��� ���� ������Ƽ�� ������
        DrawInspectableProperties(target);
    }


    private void DrawPropertyWithChildren(SerializedProperty property)
    {
        // ���� �Ӽ��� ReadOnlyAttribute�� �ִ��� Ȯ��
        Type targetType = target.GetType();
        HashSet<Type> excludedTypes = new HashSet<Type> { typeof(MonoBehaviour), typeof(object) };
        MemberInfo[] memberInfo = null;
        bool isReadOnly = false;
        bool isOnlyRuntime = false;

        // �θ� Ŭ������ ���󰡸� Ž��
        while (targetType != null &&
            !excludedTypes.Contains(targetType))
        {
            memberInfo = targetType.GetMember(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            //Debug.Log($"{property.name} in {targetType}");

            foreach (var member in memberInfo)
            {
                if (member != null)
                {
                    var readOnlyAttribute = member.GetCustomAttribute<ReadOnlyAttribute>();
                    //Debug.Log($"{member.Name} is {member.DeclaringType} and in the {target.GetType()}!");

                    if (readOnlyAttribute != null)
                    {
                        isOnlyRuntime = !Application.isPlaying && readOnlyAttribute.runtimeOnly;
                        isReadOnly = true;
                        break; // ���ϴ� Ÿ�� �� �Ӽ� �߰� �� Ž�� ����
                    }
                }
            }

            if (isReadOnly) break; // Ž�� ����

            targetType = targetType.BaseType; // �θ� Ŭ������ �̵�
        }

        if (isReadOnly)
        {
            using (new EditorGUI.DisabledScope(!isOnlyRuntime))
            {
                // ReadOnly �Ӽ��� ���� ����� ǥ��
                EditorGUILayout.PropertyField(property, true);
            }
        }
        else
        {
            // ReadOnly�� �ƴ� �Ӽ��� �⺻ ó��
            EditorGUILayout.PropertyField(property, true);
        }
    }

    // private void DrawInspectableProperties(UnityEngine.Object targetObject)
    // {
    //     var type = targetObject.GetType();
    //     var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    //     foreach (var prop in type.GetProperties(flags))
    //     {
    //         if (Attribute.IsDefined(prop, typeof(ShowPropertyInInspectorAttribute)))
    //         {
    //             object value = null;
    //             try
    //             {
    //                 value = prop.GetValue(targetObject, null);
    //             }
    //             catch (Exception e)
    //             {
    //                 Debug.LogWarning($"[ShowPropertyInInspector] {prop.Name} ���� �� ����: {e.Message}");
    //             }

    //             string valueString = value != null ? value.ToString() : "null";
    //             EditorGUILayout.LabelField(prop.Name, valueString);
    //         }
    //     }
    // }

    private void DrawInspectableProperties(UnityEngine.Object targetObject)
    {
        var type = targetObject.GetType();
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (var prop in type.GetProperties(flags))
        {
            if (Attribute.IsDefined(prop, typeof(ShowPropertyInInspectorAttribute)))
            {
                object value = null;
                try
                {
                    value = prop.GetValue(targetObject, null);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ShowPropertyInInspector] {prop.Name} ���� �� ����: {e.Message}");
                }

                string valueString = value != null ? value.ToString() : "null";
                EditorGUILayout.LabelField(prop.Name + "(Property)", valueString);
            }

            if (Attribute.IsDefined(prop, typeof(ReadOnlyAttribute)))
            {
                object value = null;
                try
                {
                    value = prop.GetValue(targetObject, null);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ReadOnly] {prop.Name} ���� �� ����: {e.Message}");
                }

                string valueString = value != null ? value.ToString() : "null";
                EditorGUILayout.LabelField(prop.Name, valueString);
            }
        }
    }
}