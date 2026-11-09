using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Campaign", menuName = "Breaking or Faking/Campaign")]
public class CampaignData : ScriptableObject
{
    [Tooltip("Days in the order they are played.")]
    [SerializeField] private List<DayData> days = new List<DayData>();

    public DayData GetDay(int index)
    {
        return days[Mathf.Clamp(index, 0, days.Count - 1)];
    }

    public bool IsLastDay(int index)
    {
        return index >= days.Count - 1;
    }

    // Rules in this day's rulebook that were not in the previous day's. Empty for the first day.
    public List<RuleData> GetAddedRules(int index)
    {
        return index <= 0 ? new List<RuleData>() : Difference(GetDay(index).Rules, GetDay(index - 1).Rules);
    }

    // Rules in the previous day's rulebook that are not in this day's. Empty for the first day.
    public List<RuleData> GetRemovedRules(int index)
    {
        return index <= 0 ? new List<RuleData>() : Difference(GetDay(index - 1).Rules, GetDay(index).Rules);
    }

    private static List<RuleData> Difference(IReadOnlyList<RuleData> rules, IReadOnlyList<RuleData> excluded)
    {
        List<RuleData> result = new List<RuleData>();
        foreach (RuleData rule in rules)
        {
            if (rule != null && !Contains(excluded, rule))
            {
                result.Add(rule);
            }
        }

        return result;
    }

    private static bool Contains(IReadOnlyList<RuleData> rules, RuleData rule)
    {
        foreach (RuleData candidate in rules)
        {
            if (candidate == rule)
            {
                return true;
            }
        }

        return false;
    }
}
