using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;
    
    [SerializeField] private string briefingScene = "Briefing";
    [SerializeField] private string settingsScene = "Settings";

    private void Start()
    {
        startGameBtn.onClick.AddListener(StartDay);
        settingsBtn.onClick.AddListener(Settings);
        quitBtn.onClick.AddListener(Quit);
    }

    private void StartDay()
    {
        GameSession.StartNewGame();
        SceneTransition.LoadScene(briefingScene, $"DAY {GameSession.DayIndex + 1}");
    }

    private void Settings()
    {
        SceneTransition.LoadScene(settingsScene);
    }

    private static void Quit()
    {
        Application.Quit();
    }
}
