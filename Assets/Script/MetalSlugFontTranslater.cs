using Microsoft.Unity.VisualStudio.Editor;
using System;
using UnityEngine;

public class MetalSlugFontTranslater : MonoBehaviour
{
    [ReadOnly]public SpriteRenderer spriteRenderer;
    private GameObject spriteDefault;

    [HideInInspector] public Sprite[] MS_Fonts;
    public string TestString = "MISSON COMPLATE";

    public float charSpace = 1f;
    [ReadOnly] public int charCount = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteDefault = Resources.Load<GameObject>("Prefabs/SpriteDefault");
        MS_Fonts = Resources.LoadAll<Sprite>("Sprite/MetalSlugFont");
        MakeStringImg(TestString);
    }

    void MakeStringImg(string str)
    {
        char[] chArr = str.ToCharArray();
        for (int i = 0; i < chArr.Length; i++)
        {
            MakeImgFromChar(chArr[i]);
        }
    }

    void MakeImgFromChar(char ch)
    {
        GameObject charObject = Instantiate(spriteDefault, transform.position + new Vector3(charSpace * charCount, 0, 0), Quaternion.identity, transform);
        charCount++;

        if (ch == Char.ToLower(ch) && ch == Char.ToUpper(ch))
        {
            if (ch >= '0' && ch <= '9')
            {
                charObject.GetComponent<SpriteRenderer>().sprite = MS_Fonts[41 + ch - '0'];
            }
            else if (ch == '!')
            {
                charObject.GetComponent<SpriteRenderer>().sprite = MS_Fonts[51];
            }
            else if (ch == '?')
            {
                charObject.GetComponent<SpriteRenderer>().sprite = MS_Fonts[52];
            }
            else
            {
                charObject.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        else
        {
            ch = Char.ToLower(ch);
            charObject.GetComponent<SpriteRenderer>().sprite = MS_Fonts[ch - 'a'];
        }
    }
}
