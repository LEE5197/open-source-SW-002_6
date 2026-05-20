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
        GameManager.Instance?.Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandleMainMenu()
    {
        GameManager.Instance?.Resume();
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
