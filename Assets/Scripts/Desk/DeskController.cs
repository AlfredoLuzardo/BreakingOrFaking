using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeskController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<NewsItemData> newsItems = new List<NewsItemData>();
    [SerializeField] private ScoringSettings scoringSettings;

    [Header("Document")]
    [SerializeField] private TMP_Text headlineText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private string noMoreNewsMessage = "No more news";

    [Header("Buttons")]
    [SerializeField] private Button approveButton;
    [SerializeField] private Button denyButton;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    private readonly Queue<NewsItemData> queue = new Queue<NewsItemData>();
    private readonly List<NewsItemData> approvedItems = new List<NewsItemData>();
    private readonly List<NewsItemData> deniedItems = new List<NewsItemData>();
    private NewsItemData currentItem;
    private int score;

    public IReadOnlyList<NewsItemData> ApprovedItems => approvedItems;
    public IReadOnlyList<NewsItemData> DeniedItems => deniedItems;
    public int Score => score;

    private void Start()
    {
        approveButton.onClick.AddListener(Approve);
        denyButton.onClick.AddListener(Deny);

        foreach (NewsItemData item in newsItems)
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

        if (approved)
        {
            approvedItems.Add(currentItem);
        }
        else
        {
            deniedItems.Add(currentItem);
        }

        bool correct = approved == currentItem.IsReal;
        score += correct ? scoringSettings.CorrectDecisionPoints : scoringSettings.WrongDecisionPoints;

        Debug.Log($"{(approved ? "Approved" : "Denied")} \"{currentItem.Headline}\": {(correct ? "correct" : "wrong")}. Score: {score}");

        UpdateScoreText();
        ShowNextItem();
    }

    private void ShowNextItem()
    {
        if (queue.Count == 0)
        {
            currentItem = null;
            headlineText.text = noMoreNewsMessage;
            bodyText.text = string.Empty;
            approveButton.interactable = false;
            denyButton.interactable = false;
            Debug.Log($"No more news. Approved: {approvedItems.Count}, Denied: {deniedItems.Count}, Score: {score}");
            return;
        }

        currentItem = queue.Dequeue();
        headlineText.text = currentItem.Headline;
        bodyText.text = currentItem.Body;
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
}
