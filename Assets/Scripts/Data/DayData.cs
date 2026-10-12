using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Day", menuName = "Breaking or Faking/Day")]
public class DayData : ScriptableObject
{
    [SerializeField] private int dayNumber = 1;
    [SerializeField] private List<NewsItemData> newsItems = new List<NewsItemData>();
    [SerializeField] private List<RuleData> rules = new List<RuleData>();

    public int DayNumber => dayNumber;
    public IReadOnlyList<NewsItemData> NewsItems => newsItems;
    public IReadOnlyList<RuleData> Rules => rules;
}
