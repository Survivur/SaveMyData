using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviour
{
   public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
