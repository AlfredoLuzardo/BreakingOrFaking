using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;
    
    [SerializeField] private string deskScene = "Desk"; 
    [SerializeField] private string settingsScene = "Settings";

    private void Start()
    {
        startGameBtn.onClick.AddListener(StartDay);
        settingsBtn.onClick.AddListener(Settings);
        quitBtn.onClick.AddListener(Quit);
    }

    private void StartDay()
    {
        SceneManager.LoadScene(deskScene);
    }

    private void Settings()
    {
        SceneManager.LoadScene(settingsScene);
    }

    private static void Quit()
    {
        Application.Quit();
    }
}
