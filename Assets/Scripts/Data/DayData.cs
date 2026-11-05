using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Day", menuName = "Breaking or Faking/Day")]
public class DayData : ScriptableObject
{
    [SerializeField] private int dayNumber = 1;
    [Tooltip("Optional message shown on the briefing screen before this day starts.")]
    [TextArea(2, 6)]
    [SerializeField] private string briefing;
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    [SerializeField] private List<RuleData> rules = new List<RuleData>();

    public int DayNumber => dayNumber;
    public string Briefing => briefing;
    public IReadOnlyList<ItemData> Items => items;
    public IReadOnlyList<RuleData> Rules => rules;
}
