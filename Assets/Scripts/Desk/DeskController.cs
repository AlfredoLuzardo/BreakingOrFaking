using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DeskController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CampaignData campaign;
    [SerializeField] private ScoringSettings scoringSettings;

    [Header("Desk")]
    [FormerlySerializedAs("documentView")]
    [SerializeField] private ArticleView articleView;
    [Tooltip("The free area evidence documents are placed on and can be dragged around.")]
    [SerializeField] private DeskSurface deskSurface;
    [Tooltip("One prefab per evidence type, e.g. PhotographView. The first one that can show a document is used.")]
    [SerializeField] private List<DocumentView> evidenceViewPrefabs = new List<DocumentView>();
    [SerializeField] private RulebookView rulebookView;
    [SerializeField] private ContradictionChecker contradictionChecker;
    [SerializeField] private string noMoreNewsMessage = "No more news";

    [Header("Buttons")]
    [SerializeField] private Button approveButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private Button endDayButton;
    [SerializeField] private string resultsSceneName = "Results";
    [SerializeField] private DocumentStamp documentStamp;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text checksText;
    [SerializeField] private Color pointsGainedColor = new Color(0.35f, 0.85f, 0.45f);
    [SerializeField] private Color pointsLostColor = new Color(0.95f, 0.35f, 0.3f);
    [SerializeField] private float popupDuration = 1f;
    [SerializeField] private float popupRise = 30f;

    private readonly Queue<StoryCase> queue = new Queue<StoryCase>();
    private readonly List<DocumentView> evidenceViews = new List<DocumentView>();
    private StoryCase currentCase;
    private bool isStamping;
    private readonly HashSet<(Section, RuleData)> foundContradictions = new HashSet<(Section, RuleData)>();
    private int wrongChecks;

    private void Start()
    {
        int dayIndex = GameSession.DayIndex;
        DayData day = campaign.GetDay(dayIndex);
        GameSession.BeginDay(day.DayNumber);
        SetUpButtons();
        contradictionChecker.Register(articleView);
        rulebookView.Show(day.Rules, campaign.GetAddedRules(dayIndex));
        EnqueueCases(day);
        UpdateScoreText();
        UpdateChecksText();
        ShowNextCase();
    }

    private void SetUpButtons()
    {
        approveButton.onClick.AddListener(() => Decide(true));
        denyButton.onClick.AddListener(() => Decide(false));
        endDayButton.onClick.AddListener(EndDay);
        endDayButton.gameObject.SetActive(false);
        contradictionChecker.Checked += OnChecked;
    }

    private void EnqueueCases(DayData day)
    {
        foreach (StoryCase storyCase in day.Cases)
        {
            if (storyCase != null)
            {
                queue.Enqueue(storyCase);
            }
        }
    }

    private void Decide(bool approved)
    {
        if (currentCase == null || isStamping)
        {
            return;
        }

        DecisionRecord decision = new DecisionRecord(currentCase, approved);
        int points = decision.IsCorrect ? scoringSettings.CorrectDecisionPoints : scoringSettings.WrongDecisionPoints;
        GameSession.AddDecision(decision, points);

        Debug.Log($"{(approved ? "Approved" : "Denied")} \"{currentCase.name}\": {(decision.IsCorrect ? "correct" : "wrong")}. Score: {GameSession.DayScore}");

        UpdateScoreText();
        ShowPopup(points);

        if (documentStamp != null)
        {
            StartCoroutine(StampThenShowNext(approved));
        }
        else
        {
            ShowNextCase();
        }
    }
    
    private IEnumerator StampThenShowNext(bool approved)
    {
        isStamping = true;
        SetDecisionButtonsInteractable(false);
        contradictionChecker.Clear();

        yield return documentStamp.Play(approved);

        documentStamp.Hide();
        SetDecisionButtonsInteractable(true);
        isStamping = false;
        ShowNextCase();
    }

    private void ShowNextCase()
    {
        contradictionChecker.Clear();
        ClearEvidence();

        if (queue.Count == 0)
        {
            ShowEndOfDay();
            return;
        }

        currentCase = queue.Dequeue();
        articleView.Show(currentCase.Article);
        SpawnEvidence(currentCase);
    }
    
    private void ShowEndOfDay()
    {
        currentCase = null;
        articleView.ShowMessage(noMoreNewsMessage);
        approveButton.gameObject.SetActive(false);
        denyButton.gameObject.SetActive(false);
        endDayButton.gameObject.SetActive(true);
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
    
    private void SpawnEvidence(StoryCase storyCase)
    {
        foreach (DocumentData document in storyCase.Evidence)
        {
            DocumentView prefab = FindViewPrefab(document);
            if (prefab == null || deskSurface == null)
            {
                Debug.LogWarning($"{storyCase.name}: can't show {document}. Check Desk Surface and Evidence View Prefabs.", this);
                continue;
            }

            DocumentView view = deskSurface.Place(prefab, evidenceViews.Count);
            view.Show(document);
            contradictionChecker.Register(view);
            evidenceViews.Add(view);
        }
    }
    
    private DocumentView FindViewPrefab(DocumentData document)
    {
        if (document == null)
        {
            return null;
        }

        return evidenceViewPrefabs.Find(prefab => prefab != null && prefab.CanShow(document));
    }

    private void ClearEvidence()
    {
        foreach (DocumentView view in evidenceViews)
        {
            contradictionChecker.Unregister(view);
            Destroy(view.gameObject);
        }

        evidenceViews.Clear();
    }

    private void SetDecisionButtonsInteractable(bool interactable)
    {
        approveButton.interactable = interactable;
        denyButton.interactable = interactable;
    }
    
    private void OnChecked(SectionView section, RuleView rule, bool contradicts)
    {
        if (contradicts)
        {
            if (foundContradictions.Add((section.Section, rule.Rule)))
            {
                GameSession.AddContradictionFound(scoringSettings.ContradictionFoundPoints);
                ShowPoints(scoringSettings.ContradictionFoundPoints);
            }

            return;
        }

        wrongChecks++;
        UpdateChecksText();

        int penalty = wrongChecks > scoringSettings.FreeWrongChecksPerDay ? scoringSettings.WrongCheckPoints : 0;
        GameSession.AddWrongCheck(penalty);
        ShowPoints(penalty);
    }

    private void ShowPoints(int points)
    {
        UpdateScoreText();
        ShowPopup(points);
    }

}
