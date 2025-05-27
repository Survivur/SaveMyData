using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void ExitGame()
    {
        #if UNITY_EDITOR
        // ����Ƽ �����Ϳ����� �÷��� ��� ����
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // ���� ����� ���ӿ����� ���ø����̼� ����
        Application.Quit();
        #endif
    }
}
