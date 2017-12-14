using UnityEngine;

[ExecuteInEditMode]
public class SizeMatcher : MonoBehaviour
{
    public Vector2 referenceSize;
    public Vector2 referencePosition;
    public int referenceFontSize;

    private Vector2 mParentReferenceResolution;
    private RectTransform mTarget;

    void Start()
    {
        mTarget = GetComponent<RectTransform>();
        mParentReferenceResolution = GetComponentInParent<BackgroundScaler>().referenceResolution;
    }

    public void Fix(Vector2 parentCurrent)
    {
        mTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, referenceSize.x * (parentCurrent.x / mParentReferenceResolution.x));
        mTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, referenceSize.y * (parentCurrent.y / mParentReferenceResolution.y));
        mTarget.localPosition = new Vector3(referencePosition.x * (parentCurrent.x / mParentReferenceResolution.x), referencePosition.y * (parentCurrent.y / mParentReferenceResolution.y));
    }
}