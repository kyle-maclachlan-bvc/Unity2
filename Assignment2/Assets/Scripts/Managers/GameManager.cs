using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private bool _isPaused = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
        _isPaused = false;
        Time.timeScale = 1f;
    }

    private void PauseGame()
    {
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
