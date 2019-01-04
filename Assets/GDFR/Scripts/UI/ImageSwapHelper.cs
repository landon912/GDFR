using System.Collections.Generic;
using UnityEngine;

public class ImageSwapHelper : MonoBehaviour
{
    public Sprite[] sprites;

    public Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    public void Build()
    {
        //build sprite dictionary
        foreach (Sprite s in sprites)
        {
            spriteDict.Add(s.name, s);
        }
    }
}