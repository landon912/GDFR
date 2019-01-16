using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GDFR
{
    public class Avatar : MonoBehaviour
    {
        public Image avatarSprite = null;
        public Image avatarGlowSprite = null;
        public TextMeshProUGUI avatarLabel = null;

        public string _name = "Avatar";

        public string Name
        {
            set
            {
                _name = value;
                avatarLabel.text = _name;
            }
            get { return _name; }
        }

        public string _spriteName = "Avatar_01";

        public string spriteName
        {
            set
            {
                _spriteName = value;
                avatarSprite.sprite = GameController.Instance.spriteSwapper.spriteDict[_spriteName];
            }
            get { return _spriteName; }
        }
    }
}