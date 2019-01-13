using UnityEngine;

namespace GDFR
{
    public class ButtonPressedLinker : MonoBehaviour
    {
        public void DispathButtonPressedEvent()
        {
            EventReceiver.TriggerButtonPressedEvent();
        }
    }
}