using UnityEngine;

// Awake�� Reset �޼ҵ带 �����ϴ� �߻� Ŭ����
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
