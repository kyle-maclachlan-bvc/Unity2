using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject pausedUI;
    [SerializeField] private TMP_Text pausedText;
    
    [SerializeField] private Transform clearPosition;   // where the Player will stand after clearing level.
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerAnimator playerAnimator;
    
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
        AudioManager.Instance.RestorMusicAfterPause();
        pausedText.SetText("Game Paused");
        _isPaused = true;
        Time.timeScale = 0f;
        
    }
    
    IEnumerator WinSequence()
    {
        //Debug.Log("You cleared the level!");
                        
        player.MoveToPosition(clearPosition.position);
        LevelClear.Instance.ShowLevelClear("You cleared the level!");
        yield return new WaitForSeconds(1.5f);
        playerAnimator.PlayCheer();
        AudioManager.Instance.FadeOutMusic(5f);
    }
    
    public void Win()
    {
        StartCoroutine(WinSequence());
    }
    
    
    
    
}
