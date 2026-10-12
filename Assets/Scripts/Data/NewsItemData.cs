using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewsItem", menuName = "Breaking or Faking/News Item")]
public class NewsItemData : ScriptableObject
{
    [SerializeField] private List<NewsSection> sections = new List<NewsSection>();

    public IReadOnlyList<NewsSection> Sections => sections;
    public bool IsReal => sections.TrueForAll(section => section.IsTrue);

    // Text of the first Title section, or the asset name if there is none.
    public string Title
    {
        get
        {
            foreach (NewsSection section in sections)
            {
                if (section.Style == NewsSectionStyle.Title)
                {
                    return section.Text;
                }
            }

            return name;
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            if (!sections[i].IsTrue && sections[i].ContradictedRules.Count == 0)
            {
                Debug.LogWarning($"{name}: section {i} is marked false but has no contradicted rules.", this);
            }
        }
    }
}
