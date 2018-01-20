using UnityEngine;

public class AvatarButton : MonoBehaviour
{
    [HideInInspector]
    public int id;

    public void OnClick()
    {
        GetComponentInParent<AvatarSelector>().AvatarSelected(id);
    }
}