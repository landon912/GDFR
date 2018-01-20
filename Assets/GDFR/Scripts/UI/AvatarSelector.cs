using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class AvatarSelector : UITable
{
    public GameObject avatarPrefab;
    public GameObject contentParent;
    public GameObject background;

    private PlayerProfile_UI mParent;
    private GameObject mBlocker;

    void Start()
    {
        mParent = GetComponentInParent<PlayerProfile_UI>();

        CreateChildren();

        BuildGrid(contentParent.transform);
    }

    void CreateChildren()
    {
        foreach (AvatarOption avatar in mParent.AvatarOptions)
        {
            GameObject child = Instantiate(avatarPrefab);
            child.transform.SetParent(contentParent.transform);

            child.GetComponent<Image>().sprite = avatar.graphic;
            child.GetComponent<AvatarButton>().id = avatar.id;
        }
    }

    public void AvatarSelected(int id)
    {
        mParent.ChangeAvatar(id);
        Hide();
    }

    public void Show()
    {
        mBlocker = CreateBlocker(transform.parent.GetComponentInParent<Canvas>(), GetComponent<Canvas>());
        contentParent.SetActive(true);
        background.SetActive(true);
    }

    public void Hide()
    {
        Destroy(mBlocker);
        contentParent.SetActive(false);
        background.SetActive(false);
    }

    protected virtual GameObject CreateBlocker(Canvas rootCanvas, Canvas selectorCanvas)
    {
        // Create blocker GameObject.
        GameObject blocker = new GameObject("Blocker");

        // Setup blocker RectTransform to cover entire root canvas area.
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.SetParent(rootCanvas.transform, false);
        blockerRect.anchorMin = Vector3.zero;
        blockerRect.anchorMax = Vector3.one;
        blockerRect.sizeDelta = Vector2.zero;

        // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
        Canvas blockerCanvas = blocker.AddComponent<Canvas>();
        blockerCanvas.overrideSorting = true;
        blockerCanvas.sortingLayerID = selectorCanvas.sortingLayerID;
        blockerCanvas.sortingOrder = selectorCanvas.sortingOrder - 1;

        // Add raycaster since it's needed to block.
        blocker.AddComponent<GraphicRaycaster>();

        // Add image since it's needed to block, but make it clear.
        Image blockerImage = blocker.AddComponent<Image>();
        blockerImage.color = Color.clear;

        // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
        Button blockerButton = blocker.AddComponent<Button>();
        blockerButton.onClick.AddListener(Hide);

        return blocker;
    }
}