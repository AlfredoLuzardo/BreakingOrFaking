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
        background.color = decision.IsCorrect ? correctColor : wrongColor;

        StringBuilder text = new StringBuilder();
        AppendSummary(text, decision);
        if (!AppendContradictions(text, decision.Case))
        {
            text.AppendLine(decision.Case.IsTrue
                ? "Nothing contradicts the rules."
                : "This story should have been denied.");
        }

        label.text = text.ToString().TrimEnd();
    }

    private static void AppendSummary(StringBuilder text, DecisionRecord decision)
    {
        text.AppendLine($"<b>{decision.Case.Title}</b>");
        text.AppendLine($"You: {(decision.Approved ? "Approved" : "Denied")}     "
            + $"Answer: {(decision.Case.IsTrue ? "Real (approve)" : "Fake (deny)")}     "
            + $"<b>{(decision.IsCorrect ? "Correct" : "Wrong")}</b>");
    }

    private static bool AppendContradictions(StringBuilder text, StoryCase storyCase)
    {
        bool found = false;
        foreach (DocumentData document in storyCase.AllDocuments())
        {
            foreach (Section section in document.GetSections())
            {
                if (section == null)
                {
                    continue;
                }

                foreach (RuleData rule in section.ContradictedRules)
                {
                    if (rule != null)
                    {
                        found = true;
                        text.AppendLine($"• {document.DescribeLocation(section)}: \"{section.Text}\" contradicts \"{rule.Text}\"");
                    }
                }
            }
        }

        return found;
    }
}
