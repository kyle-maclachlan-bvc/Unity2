using Mono.Cecil.Cil;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip treasurePickup;   // Interact with Treasure Chest SFX
    [SerializeField] private AudioClip levelClear;
    [SerializeField] private AudioClip forestAmbience;  // Audio for Forest Ambience
    [SerializeField] private AudioClip levelMusic;      // Music for Level 1
    [SerializeField] private AudioClip playerJump;      // Character SFX for jumping
    [SerializeField] private AudioClip arrowEffect;     // Character firing Arrow SFX
    [SerializeField] private AudioClip balloonPop;      // Balloon Popping
    
    [Header("Settings")]
    [Range(0, 1f)]
    [SerializeField] private float sfxVolume = .5f;
    [Range(0, 1f)]
    [SerializeField] private float musicVolume = .4f;
    [SerializeField] private AudioSource sfxSource;  // AudioSource for sound effects
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Vector2 jumpPitchRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private float pausedMusicVolume = .05f;

    private float _originalMusicVolume;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;  // 2D Audio
        
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.playOnAwake = false;
        ambientSource.loop = true;
        ambientSource.spatialBlend = 0f;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
    }

    private void Update()
    {
        ambientSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
    }

    void OnValidate()   // helps to update values in the Inspector
    {
        if (ambientSource != null)
            ambientSource.volume = sfxVolume;
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    void Start()
    {
        PlayForestAmbience();
        PlayLevelMusic();

        _originalMusicVolume = musicVolume;
    }
    
    // Public SFX Methods
    public void PlayTreasurePickup()
    {
        PlaySFX(treasurePickup);
    }

    public void PlayLevelClear()
    {
        PlaySFX(levelClear);
    }

    public void PlayJumpSFX()
    {
        if (playerJump == null) return;
        
        sfxSource.pitch = Random.Range(jumpPitchRange.x, jumpPitchRange.y);
        PlaySFX(playerJump);
    }

    public void PlayArrowSFX()
    {
        PlaySFX(arrowEffect);
    }

    public void PlayBalloonPop()
    {
        PlaySFX(balloonPop);
    }
    
    // Internal Helper
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void PlayForestAmbience()
    {
        if (forestAmbience == null) return;
        
        ambientSource.clip = forestAmbience;
        ambientSource.Play();
    }

    private void PlayLevelMusic()
    {
        if (levelMusic == null) return;
        musicSource.clip = levelMusic;
        musicSource.Play();
    }

    public void FadeOutMusic(float duration)
    {
        if (musicSource == null) return;
        musicSource.DOFade(0f, duration).OnComplete(() =>
        {
            musicSource.Stop();
        });
    }

    public void LowerMusicForPause()
    {
        musicSource.DOFade(pausedMusicVolume, 0.25f).SetUpdate(true);
    }

    public void RestorMusicAfterPause()
    {
        musicSource.DOFade(_originalMusicVolume, 0.25f).SetUpdate(true);
    }
}
