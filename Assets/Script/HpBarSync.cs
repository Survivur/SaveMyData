using UnityEngine;
using UnityEngine.UI;

public class HpBarSync : MonoBehaviour
{
    public Character character = null;

    [SerializeField, ReadOnly] private Slider slider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = character.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = (character == null) ? -666f : character.MaxHealth - character.Health;
    }
}
