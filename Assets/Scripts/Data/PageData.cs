using System;
using System.Collections.Generic;
using Desk.Document;
using UnityEngine;

[Serializable]
public class PageData
{
    [SerializeField] private PageArrangement arrangement;
    [SerializeField] private List<Section> description = new List<Section>();
    [SerializeField] private List<Sprite> images = new List<Sprite>();

    public PageArrangement Arrangement => arrangement;
    public IReadOnlyList<Section> Description => description;
    public IReadOnlyList<Sprite> Images => images;
}

[Serializable]
public class Section
{
    [SerializeField] private SectionStyle style = SectionStyle.Body;
    [SerializeField] private PageRegion region = PageRegion.Auto;
    [TextArea(2, 6)]
    [SerializeField] private string text;
    [Tooltip("Rules this section violates. The case's Is True flag determines the final answer.")]
    [SerializeField] private List<RuleData> contradictedRules = new List<RuleData>();

    public SectionStyle Style => style;
    public PageRegion Region => region;
    public string Text => text;
    public IReadOnlyList<RuleData> ContradictedRules => contradictedRules;

    public bool Contradicts(RuleData rule)
    {
        return rule != null && contradictedRules.Contains(rule);
    }
}

public enum SectionStyle
{
    Title,
    Header,
    Body
}
