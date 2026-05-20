using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ScoreSO scoreSO;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
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
    private void RestartGame() { Debug.Log("Restart Game"); }
    private void GoToMainMenu() { Debug.Log("Go to Main Menu"); }
    private void QuitGame() { Debug.Log("Quit Game"); }
}
