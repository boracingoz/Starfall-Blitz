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
    private float currentVolume = 1f;
    private bool isMusicPaused = false;

    private void Awake()
    {
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
        currentVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        UpdateVolume();
    }

    public void SetVolume(float volume)
    {
        currentVolume = volume;
        UpdateVolume();
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }

    private void UpdateVolume()
    {
        musicSource.volume = currentVolume;
        sfxSource.volume = currentVolume;
    }

    public void PlayMainMenuMusic()
    {
        if (musicSource.clip != mainMenuMusic || !musicSource.isPlaying)
        {
            musicSource.clip = mainMenuMusic;
            musicSource.loop = true;
            musicSource.Play();
            isMusicPaused = false;
        }
    }

    public void PlayRandomGameplayMusic()
    {
        if (gameplayMusic.Length == 0) return;

        AudioClip selectedClip = gameplayMusic[Random.Range(0, gameplayMusic.Length)];
        musicSource.clip = selectedClip;
        musicSource.loop = true;
        musicSource.Play();
        isMusicPaused = false;
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            isMusicPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isMusicPaused && !musicSource.isPlaying)
        {
            musicSource.UnPause();
            isMusicPaused = false;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        isMusicPaused = false;
    }

    public bool IsPlayingGameplayMusic()
    {
        if (gameplayMusic.Length == 0) return false;

        for (int i = 0; i < gameplayMusic.Length; i++)
        {
            if (musicSource.clip == gameplayMusic[i])
            {
                return true;
            }
        }
        return false;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMeteorDestroySFX() => PlaySFX(meteorDestroySFX);
    public void PlayWinSFX() => PlaySFX(winSFX);
    public void PlayGameOverSFX() => PlaySFX(gameOverSFX);
    public void PlayButtonClickSFX() => PlaySFX(buttonClickSFX);
}