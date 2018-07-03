using UnityEngine;

public class DisableIfNotRepresentingClient : DisableMediaBase
{
    [SerializeField] [TypeRestriction(typeof(IRepresentClient))]
    private Object _ScriptToCheck;

    public bool enableIfAI = false;

    public IRepresentClient ScriptToCheck
    {
        get { return (IRepresentClient)_ScriptToCheck; }
    }

    void Update()
    {
        if (enableIfAI && ScriptToCheck.IsAI())
        {
            if (!mCurrentState)
                ToggleMedia(true);
            return;
        }
        if (ScriptToCheck.IsRepresentingClientId(GDFRNetworkManager.Instance.LocalConnectionId))
        {
            //am owner, enable
            if (!mCurrentState)
                ToggleMedia(true);
        }
        else
        {
            //not owner, disable
            if (mCurrentState)
                ToggleMedia(false);
        }
    }
}