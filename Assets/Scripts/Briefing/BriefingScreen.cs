using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BriefingScreen : MonoBehaviour
{
    [SerializeField] private CampaignData campaign;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button startDayButton;
    [SerializeField] private string deskSceneName = "Desk";

    private void Start()
    {
        int dayIndex = GameSession.DayIndex;
        DayData day = campaign.GetDay(dayIndex);

        headerText.text = $"Day {day.DayNumber}";

        StringBuilder text = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(day.Briefing))
        {
            text.AppendLine(day.Briefing.Trim());
            text.AppendLine();
        }

        if (dayIndex == 0)
        {
            AppendRules(text, "Today's rules", day.Rules, false);
        }
        else
        {
            List<RuleData> added = campaign.GetAddedRules(dayIndex);
            List<RuleData> removed = campaign.GetRemovedRules(dayIndex);

            if (added.Count == 0 && removed.Count == 0)
            {
                text.AppendLine("No rule changes today.");
            }

            AppendRules(text, "New rules", added, false);
            AppendRules(text, "Removed rules", removed, true);
        }

        bodyText.text = text.ToString().TrimEnd();
        startDayButton.onClick.AddListener(StartDay);
    }

    private static void AppendRules(StringBuilder text, string heading, IReadOnlyList<RuleData> rules, bool struckThrough)
    {
        if (rules.Count == 0)
        {
            return;
        }

        text.AppendLine($"<b>{heading}</b>");
        foreach (RuleData rule in rules)
        {
            if (rule != null)
            {
                text.AppendLine(struckThrough ? $"• <s>{rule.Text}</s>" : $"• {rule.Text}");
            }
        }

        text.AppendLine();
    }

    private void StartDay()
    {
        SceneTransition.LoadScene(deskSceneName);
    }
}
