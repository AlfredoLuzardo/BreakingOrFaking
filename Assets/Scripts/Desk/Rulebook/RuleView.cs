using TMPro;
using UnityEngine;

public class RuleView : SelectableItem
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private string newTagText = "NEW";
    [SerializeField] private Color newTagColor = new Color(0.8f, 0.2f, 0.15f);

    public RuleData Rule { get; private set; }

    public void Setup(RuleData rule, bool isNew = false)
    {
        Rule = rule;
        label.text = isNew
            ? $"• {rule.Text} <color=#{ColorUtility.ToHtmlStringRGB(newTagColor)}><b>{newTagText}</b></color>"
            : $"• {rule.Text}";
    }
}
