using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadSecondScene()
    {
        SceneManager.LoadScene("SecondScene"); // SecondScene으로 이동
    }
}
