using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    public readonly UnityEvent OnResumeClicked    = new UnityEvent();
    public readonly UnityEvent OnRestartClicked   = new UnityEvent();
    public readonly UnityEvent OnMainMenuClicked  = new UnityEvent();
    public readonly UnityEvent OnQuitClicked      = new UnityEvent();

    private void Awake()
    {
        Hide();

        if (resumeButton)   resumeButton.onClick.AddListener(OnResumeClicked.Invoke);
        if (restartButton)  restartButton.onClick.AddListener(OnRestartClicked.Invoke);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuClicked.Invoke);
        if (quitButton)     quitButton.onClick.AddListener(OnQuitClicked.Invoke);
    }

    private void OnDestroy()
    {
        if (resumeButton)   resumeButton.onClick.RemoveListener(OnResumeClicked.Invoke);
        if (restartButton)  restartButton.onClick.RemoveListener(OnRestartClicked.Invoke);
        if (mainMenuButton) mainMenuButton.onClick.RemoveListener(OnMainMenuClicked.Invoke);
        if (quitButton)     quitButton.onClick.RemoveListener(OnQuitClicked.Invoke);
    }

    public void Show() { if (panel) panel.SetActive(true); }
    public void Hide() { if (panel) panel.SetActive(false); }
    public bool IsVisible => panel != null && panel.activeSelf;
}
