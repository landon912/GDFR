using UnityEngine.Networking;

public class DisableOnClient : DisableMediaBase
{
    void Update()
    {
        if (NetworkServer.active)
        {
            //server, enable
            if (!mCurrentState)
                ToggleMedia(true);
        }
        else
        {
            //not server, disable
            if (mCurrentState)
                ToggleMedia(false);
        }
    }
}