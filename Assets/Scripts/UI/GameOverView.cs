using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    public readonly UnityEvent OnRestartClicked = new UnityEvent();
    public readonly UnityEvent OnMainMenuClicked = new UnityEvent();
    public readonly UnityEvent OnQuitClicked = new UnityEvent();

    private void Awake()
    {
        Hide();

        if (restartButton)  restartButton.onClick.AddListener(OnRestartClicked.Invoke);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuClicked.Invoke);
        if (quitButton)     quitButton.onClick.AddListener(OnQuitClicked.Invoke);
    }

    private void OnDestroy()
    {
        if (restartButton)  restartButton.onClick.RemoveListener(OnRestartClicked.Invoke);
        if (mainMenuButton) mainMenuButton.onClick.RemoveListener(OnMainMenuClicked.Invoke);
        if (quitButton)     quitButton.onClick.RemoveListener(OnQuitClicked.Invoke);
    }

    public void Show(int finalScore, int highScore, bool isNewRecord)
    {
        if (panel) panel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Final Score: {finalScore}";
        if (highScoreText) highScoreText.text = $"Best: {highScore}";
    }

    public void Hide()
    {
        if (panel) panel.SetActive(false);
    }
}
