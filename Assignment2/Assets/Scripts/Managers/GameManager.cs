using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject pausedUI;
    [SerializeField] private TMP_Text pausedText;
    
    [SerializeField] private Transform clearPosition;           // where the Player will stand after clearing level.
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerAnimator playerAnimator;

    [SerializeField] private int lives = 3;
    
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
        AudioManager.Instance.LowerMusicForPause();
        _isPaused = false;
        Time.timeScale = 1f;
        
    }

    private void PauseGame()
    {
        pausedUI.SetActive(true);
        AudioManager.Instance.RestoreMusicAfterPause();
        pausedText.SetText("Game Paused");
        _isPaused = true;
        Time.timeScale = 0f;
        
    }
    
    IEnumerator WinSequence()
    {
        //Debug.Log("You cleared the level!");
                        
        player.MoveToPosition(clearPosition.position);
        yield return new WaitForSeconds(2f);
        playerAnimator.PlayCheer();
        AudioManager.Instance.FadeOutMusic(5f);
        yield return new WaitForSeconds(3f);
        LoadLevel2();
    }
    
    public void Win()
    {
        StartCoroutine(WinSequence());
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void PlayerDied()
    {
        lives--;

        if (lives > 0)
        {
            RestartLevel();
        }
        else
        {
            Debug.Log("Game Over");
        }
        
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}