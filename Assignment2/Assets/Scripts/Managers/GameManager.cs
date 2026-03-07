using Unity.Cinemachine;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject pausedUI;
    [SerializeField] private TMP_Text pausedText;
    
    private bool _isPaused = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        pausedUI.SetActive(false);
    }

    public void TogglePause()
    {
        if (_isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void ResumeGame()
    {
        pausedUI.SetActive(false);
        _isPaused = false;
        Time.timeScale = 1f;
    }

    private void PauseGame()
    {
        pausedUI.SetActive(true);
        pausedText.SetText("Game Paused");
        _isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void Win()
    {
        Debug.Log("You cleared the level!");
        LevelClear.Instance.ShowLevelClear("You've cleared the level!");
        AudioManager.Instance.PlayLevelClear();
        
    }
    
    
}
