using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX")]
    public Sound dash;
    public Sound jump;
    public Sound enemyJump;
    public Sound enemyAttack;
    public Sound playerAttack;
    public Sound playerRifle;
    public Sound turretAttack;

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;

            if (backgroundMusic != null)
                PlayMusic(backgroundMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(Sound sound)
    {
        if (sound != null && sound.clip != null)
        {
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float vol)
    {
        musicSource.volume = Mathf.Clamp01(vol);
    }
}
