using UnityEngine;

public enum ScaleMode
{
    FillLargest,
    FitSmallest
}

[ExecuteInEditMode]
public class BackgroundScaler : MonoBehaviour
{
    public ScaleMode scaleMode = ScaleMode.FillLargest;
    public Vector2 referenceResolution;
    public int widthBuffer = 0;
    public int heightBuffer = 0;
    public SizeMatcher matcher;

    private RectTransform mBackgroundImage;
    private float mAspectRatio;

    void Start()
    {
        mBackgroundImage = GetComponent<RectTransform>();

        mAspectRatio = referenceResolution.x / referenceResolution.y;
    }

    void Update()
    {
        float scaledWidthBuffer = widthBuffer * (Screen.width / referenceResolution.x);
        float scaledHeightBuffer = heightBuffer * (Screen.height / referenceResolution.y);

        float screenAspectRatio = Screen.width / (float) Screen.height;

        if (scaleMode == ScaleMode.FillLargest)
        {
            if (screenAspectRatio > mAspectRatio)
            {
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    Screen.width - scaledWidthBuffer);
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    (Screen.width - scaledHeightBuffer) / mAspectRatio);

                if (matcher != null)
                    matcher.Fix(new Vector2(Screen.width, Screen.width / mAspectRatio));
            }
            else
            {
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    Screen.height - scaledHeightBuffer);
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    (Screen.height - scaledWidthBuffer) * mAspectRatio);

                if (matcher != null)
                    matcher.Fix(new Vector2(Screen.height * mAspectRatio, Screen.height));
            }
        }
        else
        {
            if (screenAspectRatio > mAspectRatio)
            {
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    Screen.height - scaledHeightBuffer);
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    (Screen.height - scaledWidthBuffer) * mAspectRatio);

                if (matcher != null)
                    matcher.Fix(new Vector2(Screen.height * mAspectRatio, Screen.height));
            }
            else
            {
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    Screen.width - scaledWidthBuffer);
                mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    (Screen.width - scaledHeightBuffer) / mAspectRatio);

                if (matcher != null)
                    matcher.Fix(new Vector2(Screen.width, Screen.width / mAspectRatio));
            }
        }
    }
}