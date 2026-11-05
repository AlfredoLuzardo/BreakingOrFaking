using TMPro;
using UnityEngine;

public class RuleView : SelectableItem
{
    [SerializeField] private TMP_Text label;

    public RuleData Rule { get; private set; }

    public void Setup(RuleData rule)
    {
        Rule = rule;
        label.text = $"• {rule.Text}";
    }
}
