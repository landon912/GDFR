using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DisableButtonOnClients : MonoBehaviour
{
    private bool mCurrentState = true;

    void Update()
    {
        if (NetworkServer.active == false)
        {
            if(mCurrentState)
                ToggleMedia(false);
        }
        else if (GDFRNetworkManager.Instance.IsLocalClientTheHost() == false)
        {
            if (mCurrentState)
                ToggleMedia(false);
        }
        else
        {
            if (!mCurrentState)
                ToggleMedia(true);
        }
    }

    private void ToggleMedia(bool state)
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