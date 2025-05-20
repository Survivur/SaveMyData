using UnityEngine;

public class BannerChecker : MonoBehaviour
{
    [ReadOnly(true)] public GameObject WinTextObject;
    [ReadOnly(true)] public GameObject LoseTextObject;

    private void Reset()
    {
        WinTextObject = GameObject.Find("Canvas").transform.Find("WinText").gameObject;
        LoseTextObject = GameObject.Find("Canvas").transform.Find("LoseText").gameObject;
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            WinTextObject.SetActive(true);
        }
        else if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            LoseTextObject.SetActive(true);
        }
    }
}
