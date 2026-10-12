using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultRowView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color correctColor = new Color(0.78f, 0.9f, 0.76f);
    [SerializeField] private Color wrongColor = new Color(0.95f, 0.76f, 0.72f);

    public void Setup(DecisionRecord decision)
    {
        NewsItemData item = decision.Item;
        background.color = decision.IsCorrect ? correctColor : wrongColor;

        StringBuilder text = new StringBuilder();
        text.AppendLine($"<b>{item.Title}</b>");
        text.AppendLine($"You: {(decision.Approved ? "Approved" : "Denied")}     "
            + $"Answer: {(item.IsReal ? "Real (approve)" : "Fake (deny)")}     "
            + $"<b>{(decision.IsCorrect ? "Correct" : "Wrong")}</b>");

        if (item.IsReal)
        {
            text.AppendLine("Nothing contradicts the rules.");
        }

        foreach (NewsSection section in item.Sections)
        {
            if (section.IsTrue)
            {
                continue;
            }

            if (section.ContradictedRules.Count == 0)
            {
                text.AppendLine($"• \"{section.Text}\" is false.");
            }

            foreach (RuleData rule in section.ContradictedRules)
            {
                text.AppendLine($"• \"{section.Text}\" contradicts \"{rule.Text}\"");
            }
        }

        label.text = text.ToString().TrimEnd();
    }
}
