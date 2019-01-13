using UnityEngine.Networking;

#pragma warning disable CS0618 //deprecation

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