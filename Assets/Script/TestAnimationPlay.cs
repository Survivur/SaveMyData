using UnityEngine;

public class TestAnimationPlay : MonoBehaviour
{
    void Start()
    {
        GetComponent<Animation>().Play();  // 클립 이름 생략 시 첫 번째 클립 재생
    }
}
