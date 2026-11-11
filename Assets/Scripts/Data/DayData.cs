using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Day", menuName = "Breaking or Faking/Day")]
public class DayData : ScriptableObject
{
    [SerializeField] private int dayNumber = 1;
    [Tooltip("Optional message shown on the briefing screen before this day starts.")]
    [TextArea(2, 6)]
    [SerializeField] private string briefing;
    [SerializeField] private List<StoryCase> cases = new List<StoryCase>();
    [SerializeField] private List<RuleData> rules = new List<RuleData>();
    [Tooltip("Weather in order during the playable day. Use two entries for one change or three for two changes.")]
    [SerializeField] private List<DailyCondition> conditions = new List<DailyCondition>();
    [Tooltip("Optional book of reference facts for this day.")]
    [SerializeField] private InfoBookData infoBook;

    public int DayNumber => dayNumber;
    public IReadOnlyList<StoryCase> Cases => cases;
    public string Briefing => briefing;
    public IReadOnlyList<RuleData> Rules => rules;
    public IReadOnlyList<DailyCondition> Conditions => conditions;
    public InfoBookData InfoBook => infoBook;
}
