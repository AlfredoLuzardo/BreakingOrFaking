using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeskController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DayData dayData;
    [SerializeField] private ScoringSettings scoringSettings;

    [Header("Desk")]
    [SerializeField] private DocumentView documentView;
    [SerializeField] private RulebookView rulebookView;
    [SerializeField] private ContradictionChecker contradictionChecker;
    [SerializeField] private string noMoreNewsMessage = "No more news";

    [Header("Buttons")]
    [SerializeField] private Button approveButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private Button endDayButton;
    [SerializeField] private string resultsSceneName = "Results";

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    private readonly Queue<NewsItemData> queue = new Queue<NewsItemData>();
    private NewsItemData currentItem;

    private void Start()
    {
        GameSession.StartDay(dayData.DayNumber);

        approveButton.onClick.AddListener(Approve);
        denyButton.onClick.AddListener(Deny);
        endDayButton.onClick.AddListener(EndDay);
        endDayButton.gameObject.SetActive(false);

        rulebookView.Show(dayData.Rules);

        foreach (NewsItemData item in dayData.NewsItems)
        {
            if (item != null)
            {
                queue.Enqueue(item);
            }
        }

        UpdateScoreText();
        ShowNextItem();
    }

    private void Approve()
    {
        Decide(true);
    }

    private void Deny()
    {
        Decide(false);
    }

    private void Decide(bool approved)
    {
        if (currentItem == null)
        {
            return;
        }

        DecisionRecord decision = new DecisionRecord(currentItem, approved);
        int points = decision.IsCorrect ? scoringSettings.CorrectDecisionPoints : scoringSettings.WrongDecisionPoints;
        GameSession.AddDecision(decision, points);

        Debug.Log($"{(approved ? "Approved" : "Denied")} \"{currentItem.name}\": {(decision.IsCorrect ? "correct" : "wrong")}. Score: {GameSession.Score}");

        UpdateScoreText();
        ShowNextItem();
    }

    private void ShowNextItem()
    {
        contradictionChecker.Clear();

        if (queue.Count == 0)
        {
            currentItem = null;
            documentView.ShowMessage(noMoreNewsMessage);
            approveButton.gameObject.SetActive(false);
            denyButton.gameObject.SetActive(false);
            endDayButton.gameObject.SetActive(true);
            return;
        }

        currentItem = queue.Dequeue();
        documentView.Show(currentItem);
    }

    private void EndDay()
    {
        SceneManager.LoadScene(resultsSceneName);
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {GameSession.Score}";
    }
}
