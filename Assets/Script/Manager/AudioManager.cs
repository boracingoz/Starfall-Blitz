using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip mainMenuMusic;
    public AudioClip[] gameplayMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip meteorDestroySFX;
    public AudioClip winSFX;
    public AudioClip gameOverSFX;
    public AudioClip buttonClickSFX;

    private const string VolumeKey = "Volume";

    private void Awake()
    {
        // Singleton kur
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        musicSource.volume = volume;
        sfxSource.volume = volume;
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }

    public void PlayMainMenuMusic()
    {
        musicSource.clip = mainMenuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayRandomGameplayMusic()
    {
        if (gameplayMusic.Length == 0) return;
        musicSource.clip = gameplayMusic[Random.Range(0, gameplayMusic.Length)];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Shortcut methods
    public void PlayMeteorDestroySFX() => PlaySFX(meteorDestroySFX);
    public void PlayWinSFX() => PlaySFX(winSFX);
    public void PlayGameOverSFX() => PlaySFX(gameOverSFX);
    public void PlayButtonClickSFX() => PlaySFX(buttonClickSFX);
}
