using UnityEngine.Networking;

public class DisableIfConnection : DisableMediaBase
{
    void Update()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            //connected, disable
            if (mCurrentState)
                ToggleMedia(false);
        }
        else
        {
            //not connected, enable
            if (!mCurrentState)
                ToggleMedia(true);
        }
    }
}