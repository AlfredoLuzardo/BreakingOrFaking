using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private string titleScene =  "Title";

    private void Start()
    {
        backBtn.onClick.AddListener(BackToTitle);
    }
    
    private void BackToTitle()
    {
        SceneManager.LoadScene(titleScene);
    }
}
