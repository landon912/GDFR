using UnityEngine;

namespace GDFR
{
    public class AvatarButton : MonoBehaviour
    {
        [HideInInspector]
        public int id;

        public void OnClick()
        {
            GetComponentInParent<AvatarSelector>().AvatarSelected(id);
        }
    }
}