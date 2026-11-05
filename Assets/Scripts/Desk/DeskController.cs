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
    [SerializeField] private DayData dayData;
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

    private readonly Queue<StoryCase> queue = new Queue<StoryCase>();
    private readonly List<DocumentView> evidenceViews = new List<DocumentView>();
    private StoryCase currentCase;
    private bool isStamping;

    private void Start()
    {
        GameSession.StartDay(dayData.DayNumber);
        SetUpButtons();
        rulebookView.Show(dayData.Rules);
        contradictionChecker.Register(articleView);
        EnqueueCases();
        UpdateScoreText();
        ShowNextCase();
    }

    private void SetUpButtons()
    {
        approveButton.onClick.AddListener(() => Decide(true));
        denyButton.onClick.AddListener(() => Decide(false));
        endDayButton.onClick.AddListener(EndDay);
        endDayButton.gameObject.SetActive(false);
    }

    private void EnqueueCases()
    {
        foreach (StoryCase storyCase in dayData.Cases)
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
        UpdateScoreText();

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

    private void EndDay()
    {
        SceneManager.LoadScene(resultsSceneName);
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {GameSession.Score}";
    }
}
