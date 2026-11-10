using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private CampaignData campaign;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text scoreText;
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
        if (GameSession.UndecidedCount > 0)
        {
            scoreText.text += $"    Undecided: {GameSession.UndecidedCount}";
        }

        foreach (DecisionRecord decision in GameSession.Decisions)
        {
            ResultRowView row = Instantiate(rowPrefab, rowContainer);
            row.Setup(decision);
        }

        nextDayButton.gameObject.SetActive(!isLastDay);
        nextDayButton.onClick.AddListener(NextDay);
        playAgainButton.onClick.AddListener(PlayAgain);
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
