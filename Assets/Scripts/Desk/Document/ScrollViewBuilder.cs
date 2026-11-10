using UnityEngine;
using UnityEngine.UI;

public static class ScrollViewBuilder
{
    public const float Margin = 30f;

    private static readonly Color TrackColor = new Color(0f, 0f, 0f, 0.1f);
    private static readonly Color HandleColor = new Color(0.16f, 0.24f, 0.25f, 0.85f);

    public static ScrollRect Build(RectTransform owner, RectTransform content)
    {
        RectTransform viewport = CreateViewport(owner);
        AttachContent(content, viewport);

        ScrollRect scrollRect = owner.gameObject.AddComponent<WheelScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 35f;
        scrollRect.verticalScrollbar = CreateScrollbar(owner);
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        return scrollRect;
    }

    // Moves the bottom edge of the visible area (and the scrollbar) up, e.g. to make room for a footer.
    public static void SetBottomInset(ScrollRect scrollRect, float bottom)
    {
        scrollRect.viewport.offsetMin = new Vector2(Margin, bottom);
        RectTransform scrollbar = (RectTransform)scrollRect.verticalScrollbar.transform;
        scrollbar.offsetMin = new Vector2(scrollbar.offsetMin.x, bottom);
    }

    private static RectTransform CreateViewport(RectTransform owner)
    {
        RectTransform viewport = CreateChild("DocumentViewport", owner, typeof(RectMask2D));
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.offsetMin = new Vector2(Margin, Margin);
        viewport.offsetMax = new Vector2(-Margin, -Margin);
        return viewport;
    }

    private static void AttachContent(RectTransform content, RectTransform viewport)
    {
        content.SetParent(viewport, false);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = Vector2.one;
        content.pivot = new Vector2(0.5f, 1f);
        content.sizeDelta = Vector2.zero;
        content.anchoredPosition = Vector2.zero;

        ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static Scrollbar CreateScrollbar(RectTransform owner)
    {
        RectTransform rect = CreateChild("DocumentScrollbar", owner, typeof(Image), typeof(Scrollbar));
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(1f, 0.5f);
        rect.offsetMin = new Vector2(-24f, Margin);
        rect.offsetMax = new Vector2(-12f, -Margin);
        rect.GetComponent<Image>().color = TrackColor;

        RectTransform slidingArea = Stretch(CreateChild("SlidingArea", rect));
        RectTransform handle = Stretch(CreateChild("Handle", slidingArea, typeof(Image)));
        Image handleImage = handle.GetComponent<Image>();
        handleImage.color = HandleColor;

        Scrollbar scrollbar = rect.GetComponent<Scrollbar>();
        scrollbar.handleRect = handle;
        scrollbar.targetGraphic = handleImage;
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        return scrollbar;
    }

    private static RectTransform CreateChild(string name, Transform parent, params System.Type[] components)
    {
        GameObject child = new GameObject(name, typeof(RectTransform));
        child.layer = parent.gameObject.layer;
        foreach (System.Type component in components)
        {
            child.AddComponent(component);
        }

        RectTransform rect = (RectTransform)child.transform;
        rect.SetParent(parent, false);
        return rect;
    }

    private static RectTransform Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }
}
