using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Article", menuName = "Breaking or Faking/Documents/Article")]
public class ArticleData : DocumentData
{
    [SerializeField] private string title;
    [SerializeField] private PageArrangement arrangement;
    [SerializeField] private List<PageData> pages = new List<PageData>();
    [SerializeField] private List<Sprite> author = new List<Sprite>();

    public string Title => title;
    public PageArrangement Arrangement => arrangement;
    public IReadOnlyList<PageData> Pages => pages;
    public IReadOnlyList<Sprite> Author => author;
    public override string Label => "Article";

    public override IEnumerable<Section> GetSections()
    {
        foreach (PageData page in pages)
        {
            if (page == null)
            {
                continue;
            }

            foreach (Section section in page.Description)
            {
                if (section != null)
                {
                    yield return section;
                }
            }
        }
    }

    public override string DescribeLocation(Section section)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] != null && Contains(pages[i], section))
            {
                return $"Page {i + 1}";
            }
        }

        return Label;
    }

    private static bool Contains(PageData page, Section section)
    {
        foreach (Section candidate in page.Description)
        {
            if (candidate == section)
            {
                return true;
            }
        }

        return false;
    }
}
