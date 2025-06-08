using System;
using TMPro;
using UnityEngine;

public class GetTextGUI : MonoBehaviour
{
    [Header("Info", order = 0)]
    Func<string> getText;

    [Header("Components", order = 0)]
    [SerializeField, ReadOnly(true)] private TextMeshProUGUI text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CodeExtensions.SetIfUnityNull(ref text, GetComponent<TextMeshProUGUI>());
    }

    // Update is called once per frame
    void Update()
    {
        text.text = getText();
    }
}
