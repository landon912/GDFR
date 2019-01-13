using UnityEngine.Networking;

#pragma warning disable CS0618 //deprecation

public class DisableOnServer : DisableMediaBase
{
    void Update()
    {
        if (NetworkServer.active)
        {
            //server disable
            if (mCurrentState)
                ToggleMedia(false);
        }
        else
        {
            //not server, enable
            if (!mCurrentState)
                ToggleMedia(true);
        }
    }
}