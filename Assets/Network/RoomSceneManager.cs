using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSceneManager : MonoBehaviour
{
    public void LoadRoomScene()
    {
        SceneManager.LoadScene("RoomScene");
    }
}
