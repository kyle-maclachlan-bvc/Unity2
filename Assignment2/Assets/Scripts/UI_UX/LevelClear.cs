using UnityEngine;
using TMPro;

public class LevelClear : MonoBehaviour
{
    public static LevelClear Instance;

    [SerializeField] private GameObject levelClearUI;
    [SerializeField] private TMP_Text levelClearText;

    

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
        
        levelClearUI.SetActive(true);
        levelClearText.SetText(textValue);
    }

    public void HideLevelClear()
    {
        levelClearUI.SetActive(false);
    }

}
