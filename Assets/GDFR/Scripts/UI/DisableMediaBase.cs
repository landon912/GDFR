using UnityEngine;
using UnityEngine.UI;

public class DisableMediaBase : MonoBehaviour
{
    protected bool mCurrentState = true;

    protected void ToggleMedia(bool state)
    {
        Button unityButton = GetComponent<Button>();
        if (unityButton == null)
        {
            UIButton NguiButton = GetComponent<UIButton>();
            if (NguiButton == null)
            {
                Dropdown uiDropdown = GetComponent<Dropdown>();
                if (uiDropdown == null)
                {
                    Toggle toggle = GetComponent<Toggle>();
                    if (toggle == null)
                    {
                        Debug.LogError("There is no media to disable on this object");
                    }
                    else
                    {
                        toggle.interactable = state;
                    }
                }
                else
                {
                    uiDropdown.interactable = state;
                }
            }
            else
            {
                NguiButton.isEnabled = state;
            }
        }
        else
        {
            unityButton.interactable = state;
        }

        mCurrentState = state;
    }
}