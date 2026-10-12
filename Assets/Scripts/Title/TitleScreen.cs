using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Button startDayButton;
    [SerializeField] private string deskSceneName = "Desk";

    private void Start()
    {
        startDayButton.onClick.AddListener(StartDay);
    }

    private void StartDay()
    {
        SceneManager.LoadScene(deskSceneName);
    }
}
