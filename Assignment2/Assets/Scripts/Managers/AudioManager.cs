using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip treasurePickup;   // Interact with Treasure Chest SFX
    [SerializeField] private AudioClip levelClear;
    
    [Header("Settings")]
    [Range(0, 1f)]
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private AudioSource audioSource;  // AudioSource

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;  // 2D Audio
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
        audioSource.PlayOneShot(clip, sfxVolume);
    }
}
