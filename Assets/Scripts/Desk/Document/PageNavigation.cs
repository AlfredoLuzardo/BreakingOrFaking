using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// The Previous / "Page 1 / 3" / Next footer at the bottom of a document.
public class PageNavigation
{
    public const float ReservedHeight = 96f;

    private static readonly Color ButtonColor = new Color(0.16f, 0.24f, 0.25f);
    private static readonly Color PageNumberColor = new Color(0.15f, 0.15f, 0.15f);

    private readonly int layer;
    private readonly TMP_FontAsset font;
    private readonly RectTransform root;
    private readonly Button previousButton;
    private readonly Button nextButton;
    private readonly TMP_Text pageNumberText;

    public PageNavigation(RectTransform owner, TMP_FontAsset font, UnityAction onPrevious, UnityAction onNext)
    {
        layer = owner.gameObject.layer;
        this.font = font;
        root = CreateRoot(owner);
        previousButton = CreateButton("PreviousPage", "Previous", 0f, 0.27f, onPrevious);
        nextButton = CreateButton("NextPage", "Next", 0.73f, 1f, onNext);

        pageNumberText = CreateLabel("PageNumber", root);
        SetHorizontalAnchors(pageNumberText.rectTransform, 0.27f, 0.73f);
        pageNumberText.color = PageNumberColor;
    }

    public bool IsVisible => root.gameObject.activeSelf;

    public void Refresh(int pageIndex, int pageCount)
    {
        root.gameObject.SetActive(pageCount > 1);
        previousButton.interactable = pageIndex > 0;
        nextButton.interactable = pageIndex + 1 < pageCount;
        pageNumberText.text = pageCount > 0 ? $"Page {pageIndex + 1} / {pageCount}" : string.Empty;
    }

    private RectTransform CreateRoot(RectTransform owner)
    {
        RectTransform rect = CreateObject("PageNavigation", owner);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.offsetMin = new Vector2(30f, 30f);
        rect.offsetMax = new Vector2(-30f, 74f);
        return rect;
    }

    private Button CreateButton(string name, string text, float left, float right, UnityAction onClick)
    {
        RectTransform rect = CreateObject(name, root, typeof(Image), typeof(Button));
        SetHorizontalAnchors(rect, left, right);

        Image background = rect.GetComponent<Image>();
        background.color = ButtonColor;

        Button button = rect.GetComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(onClick);

        TMP_Text label = CreateLabel("Label", rect);
        SetHorizontalAnchors(label.rectTransform, 0f, 1f);
        label.text = text;
        label.color = Color.white;
        return button;
    }

    private TMP_Text CreateLabel(string name, Transform parent)
    {
        TMP_Text label = CreateObject(name, parent, typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        label.font = font;
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return label;
    }

    private RectTransform CreateObject(string name, Transform parent, params System.Type[] components)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = layer;
        foreach (System.Type component in components)
        {
            gameObject.AddComponent(component);
        }

        RectTransform rect = (RectTransform)gameObject.transform;
        rect.SetParent(parent, false);
        return rect;
    }

    private static void SetHorizontalAnchors(RectTransform rect, float left, float right)
    {
        rect.anchorMin = new Vector2(left, 0f);
        rect.anchorMax = new Vector2(right, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
