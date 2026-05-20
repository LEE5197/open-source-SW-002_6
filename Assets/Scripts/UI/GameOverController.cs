using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ScoreSO scoreSO;

    [Header("View")]
    [SerializeField] private GameOverView view;

    [Header("Events")]
    [SerializeField] private GameEvent gameOverEvent;
    [Tooltip("Raise되면 SO 데이터 및 GameManager 상태가 초기화됨.")]
    [SerializeField] private GameEvent gameResetEvent;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName;

    private void Awake()
    {
        if (!view)
        {
            Debug.LogError("[GameOverController] view is not assigned");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!view) return;

        if (gameOverEvent) gameOverEvent.RegisterListener(HandleGameOver);

        view.OnRestartClicked.AddListener(HandleRestart);
        view.OnMainMenuClicked.AddListener(HandleMainMenu);
        view.OnQuitClicked.AddListener(HandleQuit);
    }

    private void OnDisable()
    {
        if (!view) return;

        if (gameOverEvent) gameOverEvent.UnregisterListener(HandleGameOver);

        view.OnRestartClicked.RemoveListener(HandleRestart);
        view.OnMainMenuClicked.RemoveListener(HandleMainMenu);
        view.OnQuitClicked.RemoveListener(HandleQuit);
    }

    private void HandleGameOver()
    {
        int finalScore = scoreSO ? scoreSO.Value : 0;
        view.Show(finalScore);
        GameManager.Instance?.NotifyGameOver();
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
}
