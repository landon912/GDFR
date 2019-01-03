using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSpriteSwaper : MonoBehaviour
{
    public Sprite initial;
    public Sprite alternative;

    private bool mIsInitial = true;
    private Image mImage;

    void Awake()
    {
        mImage = GetComponent<Image>();
    }

    public void Swap()
    {
        //initial.enabled = !mIsInitial;
        //alternative.enabled = mIsInitial;
        mImage.sprite = mIsInitial ? alternative : initial;
        mIsInitial = !mIsInitial;
    }
}