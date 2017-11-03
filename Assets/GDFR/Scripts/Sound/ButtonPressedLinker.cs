using UnityEngine;

public class ButtonPressedLinker : MonoBehaviour
{
    public void DispathButtonPressedEvent()
    {
        EventReceiver.TriggerButtonPressedEvent();
    }
}