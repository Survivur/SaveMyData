using NUnit.Framework.Constraints;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("settings")]
    [Tooltip("scroll moving speed")]
    public  float scrollSpeed;
    [Header("References")]
    public MeshRenderer aa; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        aa.material.mainTextureOffset += new Vector2(scrollSpeed*Time.deltaTime, 0);
    }
}
