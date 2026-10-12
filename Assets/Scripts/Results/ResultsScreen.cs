using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Transform rowContainer;
    [SerializeField] private ResultRowView rowPrefab;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private string deskSceneName = "Desk";

    private void Start()
    {
        headerText.text = $"Day {GameSession.DayNumber} Results";
        scoreText.text = $"Score: {GameSession.Score}";

        foreach (DecisionRecord decision in GameSession.Decisions)
        {
            ResultRowView row = Instantiate(rowPrefab, rowContainer);
            row.Setup(decision);
        }

        playAgainButton.onClick.AddListener(PlayAgain);
    }

    private void PlayAgain()
    {
        SceneManager.LoadScene(deskSceneName);
    }
}
