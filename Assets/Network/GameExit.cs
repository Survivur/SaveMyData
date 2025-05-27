using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void ExitGame()
    {
        #if UNITY_EDITOR
        // 유니티 에디터에서는 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 실제 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
        #endif
    }
}
