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
        ItemData item = decision.Item;
        background.color = decision.IsCorrect ? correctColor : wrongColor;

        StringBuilder text = new StringBuilder();
        text.AppendLine($"<b>{item.Title}</b>");
        text.AppendLine($"You: {(decision.Approved ? "Approved" : "Denied")}     "
            + $"Answer: {(item.IsTrue ? "Real (approve)" : "Fake (deny)")}     "
            + $"<b>{(decision.IsCorrect ? "Correct" : "Wrong")}</b>");

        bool hasExplanation = false;
        for (int i = 0; i < item.Pages.Count; i++)
        {
            PageData page = item.Pages[i];
            if (page == null)
            {
                continue;
            }

            foreach (Section section in page.Description)
            {
                if (section == null)
                {
                    continue;
                }

                foreach (RuleData rule in section.ContradictedRules)
                {
                    if (rule == null)
                    {
                        continue;
                    }

                    hasExplanation = true;
                    text.AppendLine($"• Page {i + 1}: \"{section.Text}\" contradicts \"{rule.Text}\"");
                }
            }
        }

        if (!hasExplanation)
        {
            text.AppendLine(item.IsTrue
                ? "Nothing contradicts the rules."
                : "This item should have been denied.");
        }

        label.text = text.ToString().TrimEnd();
    }
}
