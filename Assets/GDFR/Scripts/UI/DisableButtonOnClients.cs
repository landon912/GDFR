using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DisableButtonOnClients : NetworkBehaviour
{
    void Update()
    {
        if (!isServer)
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
                        uiDropdown.interactable = false;
                    }
                }
                else
                {
                    NguiButton.isEnabled = false;
                }
            }
            else
            {
                unityButton.interactable = false;
            }
        }
    }
}