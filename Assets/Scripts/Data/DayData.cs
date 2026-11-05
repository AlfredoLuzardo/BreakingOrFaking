using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Day", menuName = "Breaking or Faking/Day")]
public class DayData : ScriptableObject
{
    [SerializeField] private int dayNumber = 1;
    [SerializeField] private List<StoryCase> cases = new List<StoryCase>();
    [SerializeField] private List<RuleData> rules = new List<RuleData>();

    public int DayNumber => dayNumber;
    public IReadOnlyList<StoryCase> Cases => cases;
    public IReadOnlyList<RuleData> Rules => rules;
}
