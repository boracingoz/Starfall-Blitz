using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject settingsPanel;

    [Header("Audio Settings")]
    public Slider volumeSlider;
    public Toggle vibrationToggle;

    private const string LastLevelKey = "LastLevel";
    private const string VolumeKey = "Volume";
    private const string VibrationKey = "Vibration";

    private void Start()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        AudioManager.Instance.PlayMainMenuMusic();

        // Slider de�erini y�kle
        volumeSlider.value = PlayerPrefs.GetFloat(VolumeKey, 1f);
        vibrationToggle.isOn = PlayerPrefs.GetInt(VibrationKey, 1) == 1;

        // Slider'a listener ekle
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnPlayButtonPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        string levelToLoad = PlayerPrefs.HasKey(LastLevelKey) ? PlayerPrefs.GetString(LastLevelKey) : "Level1";
        SceneManager.LoadScene(levelToLoad);
    }

    public void OnSettingsButtonPressed()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackFromSettings()
    {
        AudioManager.Instance.PlayButtonClickSFX();
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnVolumeChanged(float value)
    {
        // AudioManager �zerinden ses seviyesini ayarla
        AudioManager.Instance.SetVolume(value);
        // AudioListener.volume'u kald�rd�k ��nk� �ak��ma yap�yor
    }

    public void OnVibrationToggled(bool isOn)
    {
        PlayerPrefs.SetInt(VibrationKey, isOn ? 1 : 0);
    }
}