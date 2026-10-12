using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewsSection
{
    [SerializeField] private NewsSectionStyle style = NewsSectionStyle.Body;
    [TextArea(2, 6)]
    [SerializeField] private string text;
    [SerializeField] private bool isTrue = true;

    [Tooltip("Rules this section contradicts. Only used when Is True is off.")]
    [SerializeField] private List<RuleData> contradictedRules = new List<RuleData>();

    public NewsSectionStyle Style => style;
    public string Text => text;
    public bool IsTrue => isTrue;
    public IReadOnlyList<RuleData> ContradictedRules => contradictedRules;

    public bool Contradicts(RuleData rule)
    {
        return !isTrue && rule != null && contradictedRules.Contains(rule);
    }
}
