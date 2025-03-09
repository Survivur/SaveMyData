using UnityEngine;

// Awake와 Reset 메소드를 구현하는 추상 클래스
abstract public class ARIBehaviour : MonoBehaviour
{
    abstract protected void InitalizeComponent();

    protected void Awake()
    {
        InitalizeComponent();
    }

    protected void Reset()
    {
        InitalizeComponent();
    }
}
