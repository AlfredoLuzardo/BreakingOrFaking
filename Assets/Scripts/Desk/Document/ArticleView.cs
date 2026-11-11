using System;
using Desk.Document;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArticleView : DocumentView
{
    private const string DateFormat = "MM/dd/yyyy";
    private const float DefaultAuthorImageHeight = 72f;
    private const float DefaultImageHeight = 220f;

    [SerializeField] private RectTransform sectionContainer;
    [SerializeField] private TMP_Text messageText;
    [Tooltip("Used when neither the page nor the article picks an arrangement. "
        + "Leave empty to stack everything in one column.")]
    [SerializeField] private PageArrangement defaultArrangement;

    private ScrollRect scrollRect;
    private PageNavigation pageNavigation;
    private ArticleData article;
    private PageArrangement arrangement;
    private int pageIndex;

    public bool HasArticle => article != null;
    public event Action ViewChanged;

    private int PageCount => article != null ? article.Pages.Count : 0;
    private PageData CurrentPage => PageCount > 0 ? article.Pages[pageIndex] : null;

    public override bool CanShow(DocumentData data)
    {
        return data is ArticleData;
    }

    public override void Show(DocumentData data)
    {
        EnsureScrollAndNavigation();
        article = data as ArticleData;
        pageIndex = 0;
        ShowCurrentPage();
    }

    public void ShowMessage(string message)
    {
        EnsureScrollAndNavigation();
        article = null;
        pageIndex = 0;
        ClearContent();
        RefreshNavigation();

        messageText.text = message;
        messageText.gameObject.SetActive(true);
        ViewChanged?.Invoke();
    }

    protected override void ClearContent()
    {
        base.ClearContent();
        arrangement = null;
        if (scrollRect != null)
        {
            scrollRect.StopMovement();
        }
    }

    private void PreviousPage()
    {
        SetPage(pageIndex - 1);
    }

    private void NextPage()
    {
        SetPage(pageIndex + 1);
    }

    private void SetPage(int index)
    {
        if (index < 0 || index >= PageCount || index == pageIndex)
        {
            return;
        }

        pageIndex = index;
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        ClearContent();
        messageText.gameObject.SetActive(false);
        RefreshNavigation();

        if (article == null)
        {
            ViewChanged?.Invoke();
            return;
        }

        PageData page = CurrentPage;
        CreateArrangement(page);
        AddArticleHeader();
        AddPageBody(page);

        if (arrangement != null)
        {
            arrangement.HideEmptyRegions();
        }

        ScrollToTop();
        ViewChanged?.Invoke();
    }

    private void CreateArrangement(PageData page)
    {
        PageArrangement prefab = ChooseArrangement(page);
        if (prefab == null)
        {
            return;
        }

        arrangement = Instantiate(prefab, sectionContainer);
        Track(arrangement.gameObject);
    }

    // The page's arrangement wins, then the article's, then the default. None means one column.
    private PageArrangement ChooseArrangement(PageData page)
    {
        if (page != null && page.Arrangement != null)
        {
            return page.Arrangement;
        }

        return article.Arrangement != null ? article.Arrangement : defaultArrangement;
    }

    private void AddArticleHeader()
    {
        Transform header = ParentFor(PageRegion.Header);
        AddText(article.Title, SectionStyle.Title, header);

        if (!string.IsNullOrWhiteSpace(article.Date))
        {
            AddText(article.FormatDate(DateFormat), SectionStyle.Body, header);
        }

        float authorHeight = arrangement != null ? arrangement.AuthorImageHeight : DefaultAuthorImageHeight;
        AddImages(article.Author, authorHeight, header);
    }

    private void AddPageBody(PageData page)
    {
        if (page == null)
        {
            return;
        }

        foreach (Section section in page.Description)
        {
            if (section != null)
            {
                AddSection(section, ParentFor(RegionOf(section)));
            }
        }

        float imageHeight = arrangement != null ? arrangement.ImageHeight : DefaultImageHeight;
        AddImages(page.Images, imageHeight, ParentFor(PageRegion.Media));
    }

    private static PageRegion RegionOf(Section section)
    {
        return section.Region == PageRegion.Auto ? PageRegion.Main : section.Region;
    }

    private Transform ParentFor(PageRegion region)
    {
        return arrangement != null ? arrangement.GetRegion(region) : sectionContainer;
    }

    // Only the current page scrolls, and every page turn starts at the top.
    private void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(sectionContainer);
        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void EnsureScrollAndNavigation()
    {
        if (scrollRect != null)
        {
            return;
        }

        RectTransform owner = (RectTransform)transform;
        scrollRect = ScrollViewBuilder.Build(owner, sectionContainer);
        pageNavigation = new PageNavigation(owner, messageText.font, PreviousPage, NextPage);
    }

    private void RefreshNavigation()
    {
        pageNavigation.Refresh(pageIndex, PageCount);
        float bottom = pageNavigation.IsVisible ? PageNavigation.ReservedHeight : ScrollViewBuilder.Margin;
        ScrollViewBuilder.SetBottomInset(scrollRect, bottom);
    }
}
