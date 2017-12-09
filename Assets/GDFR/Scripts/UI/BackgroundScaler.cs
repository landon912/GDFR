using UnityEngine;

[ExecuteInEditMode]
public class BackgroundScaler : MonoBehaviour
{
    public Vector2 referenceResolution;

    private RectTransform mBackgroundImage;
    private Resolution mOldResolution;
    private float aspectRatio;

    void Awake()
    {
        mBackgroundImage = GetComponent<RectTransform>();

        aspectRatio = referenceResolution.x / referenceResolution.y;
    }

    void Update()
    {
        if (mOldResolution.width != Screen.width && mOldResolution.height != Screen.height)
        {
            float screenAspectRatio = Screen.width / (float)Screen.height;
            if (screenAspectRatio > aspectRatio)
            {
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    Screen.width);
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    Screen.width / aspectRatio);
            }
            else
            {
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.height * aspectRatio);
            }

            mOldResolution = Screen.currentResolution;
        }
    }
}