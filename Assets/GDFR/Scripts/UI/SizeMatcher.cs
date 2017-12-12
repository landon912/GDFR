using UnityEngine;

[ExecuteInEditMode]
public class SizeMatcher : MonoBehaviour
{
    public Vector2 referenceSize;
    public Vector2 referencePosition;

    private Vector2 mParentReferenceSize;
    private RectTransform mTarget;
    //private RectTransform mParent;

    void Start()
    {
        mTarget = GetComponent<RectTransform>();
        //mParent = transform.parent.GetComponent<RectTransform>();

        mParentReferenceSize = GetComponentInParent<BackgroundScaler>().referenceResolution;
    }

    public void Fix(Vector2 parentCurrent)
    {
        mTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, referenceSize.x * (parentCurrent.x / mParentReferenceSize.x));
        mTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, referenceSize.y * (parentCurrent.y / mParentReferenceSize.y));
        mTarget.localPosition = new Vector3(referencePosition.x * (parentCurrent.x / mParentReferenceSize.x), referencePosition.y * (parentCurrent.y / mParentReferenceSize.y));
    }
}