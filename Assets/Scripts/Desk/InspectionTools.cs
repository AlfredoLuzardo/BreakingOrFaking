using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// A toggleable, circular magnifier for the documents on the desk.
public class InspectionTools : MonoBehaviour
{
    [SerializeField] private ArticleView articleView;
    [SerializeField] private TMP_FontAsset font;
    [Min(1.1f)] [SerializeField] private float magnification = 2f;
    [Min(100f)] [SerializeField] private float lensDiameter = 240f;

    private static readonly Color ButtonColor = new Color(0.17f, 0.27f, 0.29f);
    private static readonly Color ActiveColor = new Color(0.28f, 0.53f, 0.5f);
    private static readonly Color InkColor = new Color(0.98f, 0.95f, 0.85f);

    private RectTransform canvasRect;
    private RectTransform deskRect;
    private RectTransform lens;
    private RectTransform mask;
    private DocumentView sourceDocument;
    private DocumentView magnifiedDocument;
    private WheelScrollRect sourceScroll;
    private WheelScrollRect magnifiedScroll;
    private SectionView[] sourceSections;
    private SectionView[] magnifiedSections;
    private Button zoomButton;
    private Image zoomButtonImage;
    // private TMP_Text buttonLabel;
    private Texture2D discTexture;
    private Texture2D ringTexture;
    private Sprite discSprite;
    private Sprite ringSprite;
    private bool zoomMode;

    private void Start()
    {
        Canvas canvas = articleView != null ? articleView.GetComponentInParent<Canvas>() : null;
        if (canvas == null)
        {
            Debug.LogWarning("Inspection Tools needs an Article View on a Canvas.", this);
            enabled = false;
            return;
        }

        canvasRect = (RectTransform)canvas.transform;
        deskRect = articleView.transform.parent as RectTransform;
        discSprite = CreateCircleSprite(false, out discTexture);
        ringSprite = CreateCircleSprite(true, out ringTexture);
        BuildButton();
        BuildLens();
        articleView.ViewChanged += OnArticleChanged;
        RefreshButton();
    }

    private void LateUpdate()
    {
        if (!zoomMode || Mouse.current == null)
        {
            if (lens != null)
            {
                lens.gameObject.SetActive(false);
            }

            return;
        }

        Vector2 pointer = Mouse.current.position.ReadValue();
        DocumentView document = FindDocumentAt(pointer);
        if (document == null)
        {
            lens.gameObject.SetActive(false);
            return;
        }

        if (document != sourceDocument || magnifiedDocument == null)
        {
            RebuildMagnifiedDocument(document);
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, pointer, null, out Vector2 pointerLocal))
        {
            lens.gameObject.SetActive(false);
            return;
        }

        RectTransform sourceRect = (RectTransform)document.transform;
        lens.gameObject.SetActive(true);
        // Place the lens in world space: anchoredPosition is offset by the Canvas pivot.
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect, pointer, null, out Vector3 pointerWorld);
        lens.position = pointerWorld;

        Vector2 sourceScreenPoint = RectTransformUtility.WorldToScreenPoint(null, sourceRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, sourceScreenPoint, null, out Vector2 sourceLocal);

        RectTransform copyRect = (RectTransform)magnifiedDocument.transform;
        copyRect.anchoredPosition = (sourceLocal - pointerLocal) * magnification;
        copyRect.localRotation = sourceRect.localRotation;
        copyRect.localScale = sourceRect.localScale * magnification;

        if (sourceScroll != null && magnifiedScroll != null)
        {
            magnifiedScroll.verticalNormalizedPosition = sourceScroll.verticalNormalizedPosition;
        }

        // Keep evidence highlights in the lens in sync without rebuilding the page.
        for (int i = 0; i < sourceSections.Length && i < magnifiedSections.Length; i++)
        {
            Image sourceImage = sourceSections[i].GetComponent<Image>();
            Image copyImage = magnifiedSections[i].GetComponent<Image>();
            copyImage.color = sourceImage.color;
        }
    }

    private DocumentView FindDocumentAt(Vector2 pointer)
    {
        // Later siblings are drawn on top, including a photograph picked up by dragging.
        for (int i = deskRect.childCount - 1; i >= 0; i--)
        {
            Transform child = deskRect.GetChild(i);
            if (!child.gameObject.activeInHierarchy || !(child is RectTransform rect) ||
                !RectTransformUtility.RectangleContainsScreenPoint(rect, pointer, null))
            {
                continue;
            }

            DocumentView document = child.GetComponent<DocumentView>();
            if (document != null && (document != articleView || articleView.HasArticle))
            {
                return document;
            }
        }

        return null;
    }

    private void OnDestroy()
    {
        if (articleView != null)
        {
            articleView.ViewChanged -= OnArticleChanged;
        }

        if (discSprite != null) Destroy(discSprite);
        if (ringSprite != null) Destroy(ringSprite);
        if (discTexture != null) Destroy(discTexture);
        if (ringTexture != null) Destroy(ringTexture);
    }

    private void ToggleZoom()
    {
        zoomMode = !zoomMode;
        if (!zoomMode)
        {
            ClearMagnifiedDocument();
            lens.gameObject.SetActive(false);
        }

        RefreshButton();
    }

    private void OnArticleChanged()
    {
        if (zoomMode && sourceDocument != null)
        {
            ClearMagnifiedDocument();
            lens.gameObject.SetActive(false);
        }

        RefreshButton();
    }

    private void RebuildMagnifiedDocument(DocumentView document)
    {
        ClearMagnifiedDocument();
        sourceDocument = document;

        // Copy the rendered document so the lens includes photographs and article pages.
        GameObject copy = Instantiate(document.gameObject, mask);
        copy.name = "MagnifiedDocument";
        magnifiedDocument = copy.GetComponent<DocumentView>();
        magnifiedDocument.enabled = false;

        DraggableDocument drag = copy.GetComponent<DraggableDocument>();
        if (drag != null) drag.enabled = false;
        DocumentStamp stamp = copy.GetComponent<DocumentStamp>();
        if (stamp != null) stamp.enabled = false;

        CanvasGroup group = copy.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.interactable = false;

        RectTransform copyRect = (RectTransform)copy.transform;
        copyRect.anchorMin = new Vector2(0.5f, 0.5f);
        copyRect.anchorMax = copyRect.anchorMin;
        copyRect.pivot = ((RectTransform)document.transform).pivot;
        copyRect.sizeDelta = ((RectTransform)document.transform).rect.size;

        sourceScroll = document.GetComponent<WheelScrollRect>();
        magnifiedScroll = copy.GetComponent<WheelScrollRect>();
        sourceSections = VisibleSections(document.gameObject);
        magnifiedSections = VisibleSections(copy);
        Canvas.ForceUpdateCanvases();
    }

    private static SectionView[] VisibleSections(GameObject root)
    {
        List<SectionView> sections = new List<SectionView>();
        foreach (SectionView section in root.GetComponentsInChildren<SectionView>(true))
        {
            // Cleared pages stay inactive until Unity destroys them at the end of the frame.
            if (section.gameObject.activeSelf)
            {
                sections.Add(section);
            }
        }

        return sections.ToArray();
    }

    private void ClearMagnifiedDocument()
    {
        if (magnifiedDocument != null)
        {
            magnifiedDocument.gameObject.SetActive(false);
            Destroy(magnifiedDocument.gameObject);
        }

        sourceDocument = null;
        magnifiedDocument = null;
        sourceScroll = null;
        magnifiedScroll = null;
        sourceSections = null;
        magnifiedSections = null;
    }

    private void RefreshButton()
    {
        if (zoomButton == null)
        {
            return;
        }

        zoomButton.interactable = zoomMode || articleView.HasArticle;
        zoomButtonImage.color = zoomMode ? ActiveColor : ButtonColor;
        // buttonLabel.text = zoomMode ? "ZOOM ON" : "ZOOM";
    }

    private void BuildButton()
    {
        RectTransform buttonRect = CreateObject("ZoomTool", canvasRect, typeof(Image), typeof(Button));
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.zero;
        buttonRect.pivot = Vector2.zero;
        buttonRect.anchoredPosition = new Vector2(30f, 22f);
        buttonRect.sizeDelta = new Vector2(94f, 94f);

        zoomButtonImage = buttonRect.GetComponent<Image>();
        zoomButton = buttonRect.GetComponent<Button>();
        zoomButton.targetGraphic = zoomButtonImage;
        zoomButton.onClick.AddListener(ToggleZoom);

        RectTransform glass = CreateObject("MagnifyingGlass", buttonRect, typeof(Image));
        glass.anchorMin = Vector2.zero;
        glass.anchorMax = Vector2.zero;
        glass.pivot = Vector2.zero;
        glass.anchoredPosition = new Vector2(19f, 32f);
        glass.sizeDelta = new Vector2(44f, 44f);
        Image glassImage = glass.GetComponent<Image>();
        glassImage.sprite = ringSprite;
        glassImage.color = InkColor;
        glassImage.raycastTarget = false;

        RectTransform handle = CreateObject("Handle", buttonRect, typeof(Image));
        handle.anchorMin = Vector2.zero;
        handle.anchorMax = Vector2.zero;
        handle.pivot = new Vector2(0.5f, 0.5f);
        handle.anchoredPosition = new Vector2(65f, 28.5f);
        handle.sizeDelta = new Vector2(9f, 30f);
        handle.localRotation = Quaternion.Euler(0f, 0f, 45f);
        Image handleImage = handle.GetComponent<Image>();
        handleImage.color = InkColor;
        handleImage.raycastTarget = false;

        // buttonLabel = CreateLabel("Label", buttonRect);
        // buttonLabel.rectTransform.anchorMin = Vector2.zero;
        // buttonLabel.rectTransform.anchorMax = new Vector2(1f, 0f);
        // buttonLabel.rectTransform.pivot = new Vector2(0.5f, 0f);
        // buttonLabel.rectTransform.anchoredPosition = new Vector2(0f, 5f);
        // buttonLabel.rectTransform.sizeDelta = new Vector2(0f, 20f);
        // buttonLabel.fontSize = 17f;
    }

    private void BuildLens()
    {
        lens = CreateObject("ZoomLens", canvasRect, typeof(CanvasGroup));
        lens.anchorMin = Vector2.zero;
        lens.anchorMax = Vector2.zero;
        lens.pivot = new Vector2(0.5f, 0.5f);
        lens.sizeDelta = Vector2.one * lensDiameter;
        CanvasGroup group = lens.GetComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.interactable = false;

        RectTransform border = CreateObject("LensBorder", lens, typeof(Image));
        Stretch(border, 0f);
        Image borderImage = border.GetComponent<Image>();
        borderImage.sprite = ringSprite;
        borderImage.color = ButtonColor;
        borderImage.raycastTarget = false;

        mask = CreateObject("CircularMask", lens, typeof(Image), typeof(Mask));
        Stretch(mask, 7f);
        Image maskImage = mask.GetComponent<Image>();
        maskImage.sprite = discSprite;
        maskImage.color = InkColor;
        maskImage.raycastTarget = false;
        mask.GetComponent<Mask>().showMaskGraphic = true;
        lens.gameObject.SetActive(false);
        lens.SetAsLastSibling();
    }

    private TMP_Text CreateLabel(string name, Transform parent)
    {
        TMP_Text label = CreateObject(name, parent, typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        if (font != null) label.font = font;
        label.color = InkColor;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return label;
    }

    private static RectTransform CreateObject(string name, Transform parent, params System.Type[] components)
    {
        GameObject item = new GameObject(name, typeof(RectTransform));
        item.layer = parent.gameObject.layer;
        foreach (System.Type component in components) item.AddComponent(component);
        RectTransform rect = (RectTransform)item.transform;
        rect.SetParent(parent, false);
        return rect;
    }

    private static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.one * inset;
        rect.offsetMax = -Vector2.one * inset;
    }

    private static Sprite CreateCircleSprite(bool ring, out Texture2D texture)
    {
        const int size = 128;
        texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float radius = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f),
                    new Vector2(size / 2f, size / 2f)) / (size / 2f);
                bool visible = radius <= 0.98f && (!ring || radius >= 0.84f);
                pixels[y * size + x] = new Color32(255, 255, 255, visible ? (byte)255 : (byte)0);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
    }
}
