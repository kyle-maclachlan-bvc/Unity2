using UnityEngine;
using TMPro;

public class LevelClear : MonoBehaviour
{
    public static LevelClear Instance;

    [SerializeField] private GameObject levelClearUI;
    [SerializeField] private TMP_Text levelClearText;

    [SerializeField] private Transform clearPosition;   // where the Player will stand after clearing level.
    [SerializeField] private PlayerController player;

void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);

        Instance = this;
    }

    void Start()
    {
        levelClearUI.SetActive(false);
    }

    public void ShowLevelClear(string textValue)
    {
        player.MoveToPosition(clearPosition.position);
        
        levelClearUI.SetActive(true);
        levelClearText.SetText(textValue);

        AudioManager.Instance.FadeOutMusic(5f);
    }

    public void HideLevelClear()
    {
        levelClearUI.SetActive(false);
    }

}
