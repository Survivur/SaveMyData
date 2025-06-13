using UnityEngine;
using UnityEngine.UI;

public class MapImageChanger : MonoBehaviour
{
    UnityEngine.UI.Image image;
    RoomManager roomManager;

    int prevStageNum = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        roomManager = GameObjectRegistry.GetOrRegister("RoomManager").GetComponent<RoomManager>();

        prevStageNum = roomManager.StageNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevStageNum != roomManager.StageNum)
        {
            image.sprite = Resources.Load<Sprite>($"Sprite/Stage{roomManager.StageNum}Background"); //roomManager.
            prevStageNum = roomManager.StageNum;
        }
    }
}
