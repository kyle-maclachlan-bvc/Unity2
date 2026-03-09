using Mono.Cecil.Cil;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip treasurePickup;   // Interact with Treasure Chest SFX
    [SerializeField] private AudioClip levelClear;
    [SerializeField] private AudioClip forestAmbience;  // Audio for Forest Ambience
    
    [Header("Settings")]
    [Range(0, 1f)]
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private AudioSource sfxSource;  // AudioSource for sound effects
    [SerializeField] private AudioSource ambientSource;

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
    }

    void Start()
    {
        PlayForestAmbience();
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
}
