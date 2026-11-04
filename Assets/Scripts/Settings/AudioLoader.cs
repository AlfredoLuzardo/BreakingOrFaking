using UnityEngine;
using UnityEngine.Audio;

public class AudioLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        Settings.ApplyVolume(mixer, PlayerPrefs.GetFloat(Settings.VolumeKey, 1f));
    }
}
