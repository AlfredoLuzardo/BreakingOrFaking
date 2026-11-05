using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeskController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CampaignData campaign;
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
    [SerializeField] private TMP_Text checksText;
    [SerializeField] private Color pointsGainedColor = new Color(0.35f, 0.85f, 0.45f);
    [SerializeField] private Color pointsLostColor = new Color(0.95f, 0.35f, 0.3f);
    [SerializeField] private float popupDuration = 1f;
    [SerializeField] private float popupRise = 30f;

    private readonly Queue<ItemData> queue = new Queue<ItemData>();
    private readonly HashSet<(Section, RuleData)> foundContradictions = new HashSet<(Section, RuleData)>();
    private ItemData currentItem;
    private int wrongChecks;

    private void Start()
    {
        int dayIndex = GameSession.DayIndex;
        DayData day = campaign.GetDay(dayIndex);
        GameSession.BeginDay(day.DayNumber);

        approveButton.onClick.AddListener(Approve);
        denyButton.onClick.AddListener(Deny);
        endDayButton.onClick.AddListener(EndDay);
        endDayButton.gameObject.SetActive(false);
        contradictionChecker.Checked += OnChecked;

        rulebookView.Show(day.Rules, campaign.GetAddedRules(dayIndex));

        foreach (ItemData item in day.Items)
        {
            if (item != null)
            {
                queue.Enqueue(item);
            }
        }

        UpdateScoreText();
        UpdateChecksText();
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

        Debug.Log($"{(approved ? "Approved" : "Denied")} \"{currentItem.name}\": {(decision.IsCorrect ? "correct" : "wrong")}. Score: {GameSession.DayScore}");

        UpdateScoreText();
        ShowPopup(points);
        ShowNextItem();
    }

    // Scores a rulebook check: each new contradiction gains points, wrong checks cost points once the free ones are used.
    private void OnChecked(SectionView section, RuleView rule, bool contradicts)
    {
        if (contradicts)
        {
            if (foundContradictions.Add((section.Section, rule.Rule)))
            {
                AddPoints(scoringSettings.ContradictionFoundPoints);
            }

            return;
        }

        wrongChecks++;
        UpdateChecksText();

        if (wrongChecks > scoringSettings.FreeWrongChecksPerDay)
        {
            AddPoints(scoringSettings.WrongCheckPoints);
        }
    }

    private void AddPoints(int points)
    {
        GameSession.AddPoints(points);
        UpdateScoreText();
        ShowPopup(points);
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
        GameSession.FinishDay();
        SceneTransition.LoadScene(resultsSceneName);
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Day {GameSession.DayNumber}    Score: {GameSession.DayScore}";
    }

    private void UpdateChecksText()
    {
        int freeLeft = Mathf.Max(0, scoringSettings.FreeWrongChecksPerDay - wrongChecks);
        checksText.text = freeLeft > 0
            ? $"Free wrong checks left: {freeLeft}"
            : $"Wrong checks cost {-scoringSettings.WrongCheckPoints} points";
    }

    // Shows "+N" or "-N" next to the score, rising and fading out.
    private void ShowPopup(int points)
    {
        if (points == 0)
        {
            return;
        }

        TMP_Text popup = Instantiate(scoreText, scoreText.transform.parent);
        popup.name = "ScorePopup";
        popup.text = points > 0 ? $"+{points}" : points.ToString();
        popup.color = points > 0 ? pointsGainedColor : pointsLostColor;
        popup.raycastTarget = false;
        popup.rectTransform.anchoredPosition = scoreText.rectTransform.anchoredPosition
            + new Vector2(scoreText.preferredWidth + 20f, 0f);
        StartCoroutine(AnimatePopup(popup));
    }

    private IEnumerator AnimatePopup(TMP_Text popup)
    {
        Vector2 start = popup.rectTransform.anchoredPosition;
        for (float elapsed = 0f; elapsed < popupDuration; elapsed += Time.deltaTime)
        {
            float progress = elapsed / popupDuration;
            popup.rectTransform.anchoredPosition = start + new Vector2(0f, popupRise * progress);
            popup.alpha = 1f - progress * progress;
            yield return null;
        }

        Destroy(popup.gameObject);
    }
}
