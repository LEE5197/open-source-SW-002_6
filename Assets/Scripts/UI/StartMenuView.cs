using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartMenuView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    public readonly UnityEvent OnStartClicked = new UnityEvent();
    public readonly UnityEvent OnQuitClicked = new UnityEvent();

    private void Awake()
    {
        if (startButton) startButton.onClick.AddListener(OnStartClicked.Invoke);
        if (quitButton)  quitButton.onClick.AddListener(OnQuitClicked.Invoke);
    }

    private void OnDestroy()
    {
        if (startButton) startButton.onClick.RemoveListener(OnStartClicked.Invoke);
        if (quitButton)  quitButton.onClick.RemoveListener(OnQuitClicked.Invoke);
    }

    public void SetHighScore(int highScore)
    {
        if (highScoreText) highScoreText.text = $"Best: {highScore}";
    }
}
