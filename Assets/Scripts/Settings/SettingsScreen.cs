using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private string titleScene =  "Title";
    
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private Toggle fullscreenToggle;

    public const string VolumeKey = "MasterVolume";
    public const string MixerParam = "MasterVol";

    private void Start()
    {
        backBtn.onClick.AddListener(BackToTitle);
        
        volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(VolumeKey, 1f));
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }
    
    private void BackToTitle()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(titleScene);
    }

    private void SetVolume(float value)
    {
        ApplyVolume(mixer, value);
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    private void SetFullscreen(bool isOn)
    {
        Screen.fullScreen = isOn;
    }

    public static void ApplyVolume(AudioMixer mixer, float value)
    {
        mixer.SetFloat(MixerParam, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
    }
}
