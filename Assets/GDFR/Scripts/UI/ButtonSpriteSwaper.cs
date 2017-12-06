using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class ButtonSpriteSwaper : MonoBehaviour
{
    public string initial;
    public string alternative;

    private bool mIsInitial = true;
    private UIButton mButton;

    void Awake()
    {
        mButton = GetComponent<UIButton>();
    }

    public void Swap()
    {
        mButton.normalSprite = mIsInitial ? alternative : initial;
        mIsInitial = !mIsInitial;
    }
}