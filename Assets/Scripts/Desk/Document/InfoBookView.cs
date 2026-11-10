using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A closed book on the desk. Clicking it opens it to show the day's facts one page at a time.
public class InfoBookView : DocumentView, IPointerClickHandler
{
    [SerializeField] private GameObject cover;
    [SerializeField] private TMP_Text coverTitle;
    [SerializeField] private RectTransform openPages;
    [SerializeField] private RectTransform pageContent;
    [SerializeField] private Button closeButton;
    [SerializeField] private Vector2 closedSize = new Vector2(220f, 300f);
    [SerializeField] private Vector2 openSize = new Vector2(600f, 700f);

    private InfoBookData book;
    private PageNavigation pageNavigation;
    private int pageIndex;
    private bool isOpen;

    private int PageCount => book != null ? book.Pages.Count : 0;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public override bool CanShow(DocumentData data)
    {
        return data is InfoBookData;
    }

    public override void Show(DocumentData data)
    {
        book = data as InfoBookData;
        pageIndex = 0;
        coverTitle.text = book != null ? book.Title : string.Empty;
        Close();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isOpen && eventData.button == PointerEventData.InputButton.Left)
        {
            Open();
        }
    }

    private void Open()
    {
        if (book == null)
        {
            return;
        }

        isOpen = true;
        ((RectTransform)transform).sizeDelta = openSize;
        cover.SetActive(false);
        openPages.gameObject.SetActive(true);

        if (pageNavigation == null)
        {
            pageNavigation = new PageNavigation(openPages, coverTitle.font, PreviousPage, NextPage);
        }

        ShowCurrentPage();
    }

    private void Close()
    {
        isOpen = false;
        ClearContent();
        ((RectTransform)transform).sizeDelta = closedSize;
        cover.SetActive(true);
        openPages.gameObject.SetActive(false);
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
        pageNavigation.Refresh(pageIndex, PageCount);

        if (PageCount == 0)
        {
            return;
        }

        InfoPage page = book.Pages[pageIndex];
        if (!string.IsNullOrWhiteSpace(page.Heading))
        {
            AddText(page.Heading, SectionStyle.Header, pageContent);
        }

        foreach (string fact in page.Facts)
        {
            if (!string.IsNullOrWhiteSpace(fact))
            {
                AddText($"• {fact}", SectionStyle.Body, pageContent);
            }
        }
    }
}
