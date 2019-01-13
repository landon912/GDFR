using UnityEngine.Networking;

#pragma warning disable CS0618 //deprecation

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