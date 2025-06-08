using System;
using TMPro;
using UnityEngine;

public class GetTextGUI : MonoBehaviour
{
    public Func<string> getTextFunc;

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
        if (getTextFunc != null)
            text.text = getTextFunc();
    }
}
