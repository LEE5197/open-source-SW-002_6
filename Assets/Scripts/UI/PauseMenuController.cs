using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private PauseMenuView view;

    [Header("Events")]
    [Tooltip("Raise되면 SO 데이터 및 GameManager 상태가 초기화됨.")]
    [SerializeField] private GameEvent gameResetEvent;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName;

    private void Awake()
    {
        if (!view)
        {
            Debug.LogError("[PauseMenuController] view is not assigned", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!view) return;

        if (gameResetEvent) gameResetEvent.RegisterListener(HandleReset);

        view.OnResumeClicked.AddListener(HandleResume);
        view.OnRestartClicked.AddListener(HandleRestart);
        view.OnMainMenuClicked.AddListener(HandleMainMenu);
        view.OnQuitClicked.AddListener(HandleQuit);
    }

    private void OnDisable()
    {
        if (!view) return;

        if (gameResetEvent) gameResetEvent.UnregisterListener(HandleReset);

        view.OnResumeClicked.RemoveListener(HandleResume);
        view.OnRestartClicked.RemoveListener(HandleRestart);
        view.OnMainMenuClicked.RemoveListener(HandleMainMenu);
        view.OnQuitClicked.RemoveListener(HandleQuit);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;

        var gm = GameManager.Instance;
        if (gm == null) return;

        // 게임 오버/클리어 상태에서는 ESC 무시
        if (!gm.IsGameRunning) return;

        if (gm.IsPaused)
            HandleResume();
        else
            OpenPause();
    }

    private void OpenPause()
    {
        GameManager.Instance?.Pause();
        view.Show();
    }

    private void HandleResume()
    {
        GameManager.Instance?.Resume();
        view.Hide();
    }

    private void HandleRestart()
    {
        if (gameResetEvent != null) gameResetEvent.Raise();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandleMainMenu()
    {
        if (gameResetEvent != null) gameResetEvent.Raise();
        if (string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HandleQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // gameResetEvent Raise 시 (재시작/메인메뉴) 패널 강제 닫기
    private void HandleReset()
    {
        view.Hide();
    }
}
