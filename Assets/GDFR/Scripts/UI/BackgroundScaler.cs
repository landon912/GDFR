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
    //private Resolution mOldResolution;
    private float aspectRatio;


    void Start()
    {
        mBackgroundImage = GetComponent<RectTransform>();

        aspectRatio = referenceResolution.x / referenceResolution.y;

    }

    void Update()
    {

        // if (mOldResolution.width != Screen.width && mOldResolution.height != Screen.height)
        {
            float scaledWidthBuffer = widthBuffer * (Screen.width / referenceResolution.x);
            float scaledHeightBuffer = heightBuffer * (Screen.height / referenceResolution.y);

            float screenAspectRatio = Screen.width / (float)Screen.height;

            if (scaleMode == ScaleMode.FillLargest)
            {
                if (screenAspectRatio > aspectRatio)
                {
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        Screen.width - scaledWidthBuffer);
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        (Screen.width - scaledHeightBuffer) / aspectRatio);

                    if(matcher != null)
                        matcher.Fix(new Vector2(Screen.width, Screen.width / aspectRatio));
                }
                else
                {
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - scaledHeightBuffer);
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        (Screen.height - scaledWidthBuffer) * aspectRatio );

                    if (matcher != null)
                        matcher.Fix(new Vector2(Screen.height * aspectRatio, Screen.height));
                }
            }
            else
            {
                if (screenAspectRatio > aspectRatio)
                {
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        Screen.height - scaledHeightBuffer);
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        (Screen.height - scaledWidthBuffer) * aspectRatio);

                    if (matcher != null)
                        matcher.Fix(new Vector2(Screen.height * aspectRatio, Screen.height));
                }
                else
                {
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width - scaledWidthBuffer);
                    mBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        (Screen.width - scaledHeightBuffer) / aspectRatio);

                    if (matcher != null)
                        matcher.Fix(new Vector2(Screen.width, Screen.width / aspectRatio));
                }
            }

            //mOldResolution = Screen.currentResolution;
        }
    }
}