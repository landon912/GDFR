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
                    Debug.LogError("There is no button to disable on this object");
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