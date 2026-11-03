using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentView : MonoBehaviour
{
    [SerializeField] private RectTransform sectionContainer;
    [SerializeField] private SectionView sectionPrefab;
    [SerializeField] private TMP_Text messageText;

    private readonly List<GameObject> contentViews = new List<GameObject>();
    private ScrollRect scrollRect;
    private RectTransform pageNavigation;
    private Button previousPageButton;
    private Button nextPageButton;
    private TMP_Text pageNumberText;
    private ItemData currentItem;

    public int CurrentPageIndex { get; private set; }
    public int PageCount => currentItem != null ? currentItem.Pages.Count : 0;

    public event Action<SectionView> SectionClicked;
    public event Action ContentCleared;

    public void Show(ItemData item)
    {
        EnsureScrollView();
        currentItem = item;
        CurrentPageIndex = 0;
        ShowCurrentPage();
    }

    public void PreviousPage()
    {
        SetPage(CurrentPageIndex - 1);
    }

    public void NextPage()
    {
        SetPage(CurrentPageIndex + 1);
    }

    private void SetPage(int index)
    {
        if (index < 0 || index >= PageCount || index == CurrentPageIndex)
        {
            return;
        }

        CurrentPageIndex = index;
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        ClearContent();
        messageText.gameObject.SetActive(false);
        UpdatePageNavigation();

        if (currentItem == null)
        {
            return;
        }

        AddText(currentItem.Title, SectionStyle.Title);
        if (!string.IsNullOrWhiteSpace(currentItem.Date))
        {
            string dateText = currentItem.TryGetDate(out DateTime date)
                ? date.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                : currentItem.Date;
            AddText(dateText, SectionStyle.Body);
        }

        AddImages(currentItem.Author, 72f);

        PageData page = PageCount > 0 ? currentItem.Pages[CurrentPageIndex] : null;
        if (page != null)
        {
            foreach (Section section in page.Description)
            {
                if (section == null)
                {
                    continue;
                }

                SectionView view = Instantiate(sectionPrefab, sectionContainer);
                view.Setup(section);
                view.Clicked += () => SectionClicked?.Invoke(view);
                contentViews.Add(view.gameObject);
            }

            AddImages(page.Images, 220f);
        }

        // Only the current page scrolls. Every page turn starts at the top.
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(sectionContainer);
        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void ShowMessage(string message)
    {
        currentItem = null;
        CurrentPageIndex = 0;
        ClearContent();
        if (pageNavigation != null)
        {
            UpdatePageNavigation();
        }

        messageText.text = message;
        messageText.gameObject.SetActive(true);
    }

    private void UpdatePageNavigation()
    {
        bool hasMultiplePages = PageCount > 1;
        pageNavigation.gameObject.SetActive(hasMultiplePages);
        previousPageButton.interactable = CurrentPageIndex > 0;
        nextPageButton.interactable = CurrentPageIndex + 1 < PageCount;
        pageNumberText.text = PageCount > 0 ? $"Page {CurrentPageIndex + 1} / {PageCount}" : string.Empty;
        scrollRect.viewport.offsetMin = new Vector2(30f, hasMultiplePages ? 96f : 30f);
    }

    private void AddText(string text, SectionStyle style)
    {
        SectionView view = Instantiate(sectionPrefab, sectionContainer);
        view.SetupText(text, style);
        contentViews.Add(view.gameObject);
    }

    private void AddImages(IReadOnlyList<Sprite> sprites, float height)
    {
        foreach (Sprite sprite in sprites)
        {
            if (sprite == null)
            {
                continue;
            }

            GameObject imageObject = new GameObject("DocumentImage",
                typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            imageObject.layer = sectionContainer.gameObject.layer;
            imageObject.transform.SetParent(sectionContainer, false);
            Image image = imageObject.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            LayoutElement layout = imageObject.GetComponent<LayoutElement>();
            layout.minHeight = height;
            layout.preferredHeight = height;
            contentViews.Add(imageObject);
        }
    }

    private void EnsureScrollView()
    {
        if (scrollRect != null)
        {
            return;
        }

        // Keep page content clipped above the fixed navigation footer.
        GameObject viewportObject = new GameObject("DocumentViewport",
            typeof(RectTransform), typeof(RectMask2D));
        viewportObject.layer = gameObject.layer;
        RectTransform viewport = (RectTransform)viewportObject.transform;
        viewport.SetParent(transform, false);
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.offsetMin = new Vector2(30f, 96f);
        viewport.offsetMax = new Vector2(-30f, -30f);

        sectionContainer.SetParent(viewport, false);
        sectionContainer.anchorMin = new Vector2(0f, 1f);
        sectionContainer.anchorMax = Vector2.one;
        sectionContainer.pivot = new Vector2(0.5f, 1f);
        sectionContainer.sizeDelta = Vector2.zero;
        sectionContainer.anchoredPosition = Vector2.zero;

        ContentSizeFitter fitter = sectionContainer.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect = gameObject.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.content = sectionContainer;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 35f;

        CreatePageNavigation();
    }

    private void CreatePageNavigation()
    {
        GameObject navigationObject = new GameObject("PageNavigation", typeof(RectTransform));
        navigationObject.layer = gameObject.layer;
        pageNavigation = (RectTransform)navigationObject.transform;
        pageNavigation.SetParent(transform, false);
        pageNavigation.anchorMin = Vector2.zero;
        pageNavigation.anchorMax = new Vector2(1f, 0f);
        pageNavigation.pivot = new Vector2(0.5f, 0f);
        pageNavigation.offsetMin = new Vector2(30f, 30f);
        pageNavigation.offsetMax = new Vector2(-30f, 74f);

        previousPageButton = CreatePageButton("PreviousPage", "Previous", 0f, 0.27f);
        previousPageButton.onClick.AddListener(PreviousPage);
        nextPageButton = CreatePageButton("NextPage", "Next", 0.73f, 1f);
        nextPageButton.onClick.AddListener(NextPage);

        pageNumberText = CreateNavigationLabel("PageNumber", pageNavigation);
        SetHorizontalAnchors(pageNumberText.rectTransform, 0.27f, 0.73f);
        pageNumberText.color = new Color(0.15f, 0.15f, 0.15f);
    }

    private Button CreatePageButton(string name, string text, float left, float right)
    {
        GameObject buttonObject = new GameObject(name,
            typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.layer = gameObject.layer;
        buttonObject.transform.SetParent(pageNavigation, false);
        SetHorizontalAnchors((RectTransform)buttonObject.transform, left, right);

        Image background = buttonObject.GetComponent<Image>();
        background.color = new Color(0.16f, 0.24f, 0.25f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = background;
        TMP_Text label = CreateNavigationLabel("Label", buttonObject.transform);
        SetHorizontalAnchors(label.rectTransform, 0f, 1f);
        label.text = text;
        label.color = Color.white;
        return button;
    }

    private TMP_Text CreateNavigationLabel(string name, Transform parent)
    {
        GameObject labelObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.layer = gameObject.layer;
        labelObject.transform.SetParent(parent, false);
        TMP_Text label = labelObject.GetComponent<TMP_Text>();
        label.font = messageText.font;
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return label;
    }

    private static void SetHorizontalAnchors(RectTransform rect, float left, float right)
    {
        rect.anchorMin = new Vector2(left, 0f);
        rect.anchorMax = new Vector2(right, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void ClearContent()
    {
        ContentCleared?.Invoke();
        foreach (GameObject view in contentViews)
        {
            view.SetActive(false);
            Destroy(view);
        }

        contentViews.Clear();
        if (scrollRect != null)
        {
            scrollRect.StopMovement();
        }
    }
}
