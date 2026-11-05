using Desk.Document;
using UnityEngine;
// Decides where things sit on a page, nothing else: no colours, fonts or decoration.
// Build it as a prefab with up to four region containers. Each region needs a
// VerticalLayoutGroup (Control Child Size on, Child Force Expand Height off).
// Missing regions fall back to Main, and Main falls back to the root.
public class PageArrangement : MonoBehaviour
{
    [SerializeField] private RectTransform header;
    [SerializeField] private RectTransform media;
    [SerializeField] private RectTransform main;
    [SerializeField] private RectTransform side;

    [SerializeField] private float imageHeight = 220f;
    [SerializeField] private float authorImageHeight = 72f;

    public float ImageHeight => imageHeight;
    public float AuthorImageHeight => authorImageHeight;

    public RectTransform GetRegion(PageRegion region)
    {
        RectTransform target = region switch
        {
            PageRegion.Header => header,
            PageRegion.Media => media,
            PageRegion.Main => main,
            PageRegion.Side => side,
            _ => null
        };

        if (target != null)
        {
            return target;
        }

        return main != null ? main : (RectTransform)transform;
    }

    public void HideEmptyRegions()
    {
        HideIfEmpty(header);
        HideIfEmpty(media);
        HideIfEmpty(main);
        HideIfEmpty(side);
    }

    private void HideIfEmpty(RectTransform region)
    {
        if (region == null || region == transform)
        {
            return;
        }

        foreach (Transform child in region)
        {
            if (child.gameObject.activeSelf)
            {
                return;
            }
        }

        region.gameObject.SetActive(false);
    }
}
