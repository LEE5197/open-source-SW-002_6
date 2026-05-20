using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ScoreSO scoreSO;

    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Events")]
    [SerializeField] private GameEvent gameOverEvent;
    private void Awake()
    {
        // 시작 시 패널이 꺼져있도록 보장
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (gameOverEvent != null)
        {
            gameOverEvent.RegisterListener(OnGameOver);
        }
    }

    private void OnDisable()
    {
        if (gameOverEvent != null)
        {
            gameOverEvent.UnregisterListener(OnGameOver);
        }
    }

    public void OnGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        Time.timeScale = 0f; // 게임 시간 정지
        ShowGameOverUI();
    }

    public void ShowGameOverUI()
    {
        if (scoreSO != null && finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {scoreSO.Value}";
        }
        
        // 버튼 클릭 이벤트는 나중에 구현할 수 있도록 슬롯만 확보
        // restartButton.onClick.AddListener(RestartGame);
        // mainMenuButton.onClick.AddListener(GoToMainMenu);
        // quitButton.onClick.AddListener(QuitGame);
    }

    // 나중에 구현할 메서드들 (작동 안함)
    public void RestartGame() 
    { 
        Time.timeScale = 1f; // 재시작 전 시간 복구 필수
        Debug.Log("Restart Game"); 
    }
    
    public void GoToMainMenu() 
    { 
        Time.timeScale = 1f; 
        Debug.Log("Go to Main Menu"); 
    }
    
    public void QuitGame() { Debug.Log("Quit Game"); }
}
