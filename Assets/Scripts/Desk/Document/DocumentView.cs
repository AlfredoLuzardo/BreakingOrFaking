using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DocumentView : MonoBehaviour
{
    [SerializeField] private SectionView sectionPrefab;

    private readonly List<GameObject> contentViews = new List<GameObject>();

    public event Action<SectionView> SectionClicked;
    public event Action ContentCleared;

    public abstract bool CanShow(DocumentData data);
    public abstract void Show(DocumentData data);

    protected virtual void ClearContent()
    {
        ContentCleared?.Invoke();
        foreach (GameObject view in contentViews)
        {
            view.SetActive(false);
            Destroy(view);
        }

        contentViews.Clear();
    }

    protected void Track(GameObject view)
    {
        contentViews.Add(view);
    }

    protected SectionView AddSection(Section section, Transform parent)
    {
        SectionView view = Instantiate(sectionPrefab, parent);
        view.Setup(section);
        view.Clicked += () => SectionClicked?.Invoke(view);
        Track(view.gameObject);
        return view;
    }

    protected SectionView AddText(string text, SectionStyle style, Transform parent)
    {
        SectionView view = Instantiate(sectionPrefab, parent);
        view.SetupText(text, style);
        Track(view.gameObject);
        return view;
    }

    protected void AddImages(IReadOnlyList<Sprite> sprites, float height, Transform parent)
    {
        foreach (Sprite sprite in sprites)
        {
            if (sprite != null)
            {
                AddImage(sprite, height, parent);
            }
        }
    }

    protected Image AddImage(Sprite sprite, float height, Transform parent)
    {
        GameObject imageObject = new GameObject("DocumentImage",
            typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        imageObject.layer = parent.gameObject.layer;
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        LayoutElement layout = imageObject.GetComponent<LayoutElement>();
        layout.minHeight = height;
        layout.preferredHeight = height;

        Track(imageObject);
        return image;
    }
}
