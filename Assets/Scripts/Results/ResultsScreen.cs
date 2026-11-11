using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private CampaignData campaign;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text breakdownText;
    [SerializeField] private Transform rowContainer;
    [SerializeField] private ResultRowView rowPrefab;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private string briefingSceneName = "Briefing";

    private void Start()
    {
        bool isLastDay = campaign.IsLastDay(GameSession.DayIndex);

        headerText.text = isLastDay
            ? $"Day {GameSession.DayNumber} Results - Final Day"
            : $"Day {GameSession.DayNumber} Results";
        scoreText.text = isLastDay
            ? $"Day score: {GameSession.DayScore}    Final total: {GameSession.TotalScore}"
            : $"Day score: {GameSession.DayScore}    Total: {GameSession.TotalScore}";
        breakdownText.text = BuildBreakdown(isLastDay);

        foreach (DecisionRecord decision in GameSession.Decisions)
        {
            ResultRowView row = Instantiate(rowPrefab, rowContainer);
            row.Setup(decision);
        }

        nextDayButton.gameObject.SetActive(!isLastDay);
        nextDayButton.onClick.AddListener(NextDay);
        playAgainButton.onClick.AddListener(PlayAgain);
    }

    // Lists where the day's points came from, with the points in a second column.
    private static string BuildBreakdown(bool isLastDay)
    {
        int freeWrongChecks = GameSession.WrongChecks - GameSession.PenalizedWrongChecks;

        StringBuilder text = new StringBuilder();
        text.AppendLine("<b>Points</b>");
        text.AppendLine();
        AppendRow(text, $"Correct calls ({GameSession.CorrectCalls})", Signed(GameSession.CorrectCallPoints));
        AppendRow(text, $"Wrong calls ({GameSession.WrongCalls})", Signed(GameSession.WrongCallPoints));
        AppendRow(text, $"Contradictions found ({GameSession.ContradictionsFound})", Signed(GameSession.ContradictionPoints));
        AppendRow(text, $"Wrong checks ({GameSession.WrongChecks}, {freeWrongChecks} free)", Signed(GameSession.WrongCheckPoints));
        text.AppendLine();
        AppendRow(text, "<b>Day score</b>", $"<b>{GameSession.DayScore}</b>");
        AppendRow(text, isLastDay ? "Final total" : "Total", GameSession.TotalScore.ToString());
        return text.ToString().TrimEnd();
    }

    private static void AppendRow(StringBuilder text, string label, string points)
    {
        text.AppendLine($"{label}<pos=80%>{points}");
    }

    private static string Signed(int points)
    {
        return points > 0 ? $"+{points}" : points.ToString();
    }

    private void NextDay()
    {
        GameSession.AdvanceToNextDay();
        SceneTransition.LoadScene(briefingSceneName, $"DAY {GameSession.DayIndex + 1}");
    }

    private void PlayAgain()
    {
        GameSession.StartNewGame();
        SceneTransition.LoadScene(briefingSceneName, $"DAY {GameSession.DayIndex + 1}");
    }
}
