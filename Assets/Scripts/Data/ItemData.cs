using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Breaking or Faking/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private PageArrangement arrangement;
    [SerializeField] private List<PageData> pages = new List<PageData>();
    [SerializeField] private List<Sprite> author = new List<Sprite>();
    [Tooltip("Publication date as eight digits: MMDDYYYY (for example, 10312026).")]
    [SerializeField] private string date;
    [Tooltip("The answer used for approve/deny scoring, independent of the section rule links.")]
    [SerializeField] private bool isTrue = true;

    public string Title => title;
    public PageArrangement Arrangement => arrangement;
    public IReadOnlyList<PageData> Pages => pages;
    public IReadOnlyList<Sprite> Author => author;
    public string Date => date;
    public bool IsTrue => isTrue;

    public bool TryGetDate(out DateTime value)
    {
        return DateTime.TryParseExact(date, "MMddyyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out value);
    }

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(date) && !TryGetDate(out _))
        {
            Debug.LogWarning($"{name}: Date must be a valid date in MMDDYYYY format.", this);
        }

        bool hasContradictions = false;
        foreach (PageData page in pages)
        {
            if (page == null)
            {
                continue;
            }

            foreach (Section section in page.Description)
            {
                if (section == null)
                {
                    continue;
                }

                foreach (RuleData rule in section.ContradictedRules)
                {
                    hasContradictions |= rule != null;
                }
            }
        }

        if (isTrue && hasContradictions)
        {
            Debug.LogWarning($"{name}: a true item contains section rule violations. Check Is True and the rule links.", this);
        }
        else if (!isTrue && pages.Count > 0 && !hasContradictions)
        {
            Debug.LogWarning($"{name}: a false item has no section rule links to explain its answer.", this);
        }
    }
}
