using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumenSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolumen"))
        {
            LoadVolumen();
        }
        else
        {
            SetMusicVolumen();
        }

        SetMusicVolumen();
    }

    public void SetMusicVolumen()
    {
        float volumen = musicSlider.value;
        myMixer.SetFloat("musica", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("musicVolumen", volumen);
    }

    private void LoadVolumen()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolumen");
        SetMusicVolumen();
    }
}
