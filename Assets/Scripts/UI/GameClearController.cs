using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ScoreSO scoreSO;
    [SerializeField] private HighScoreSO highScoreSO;

    [Header("View")]
    [SerializeField] private GameOverView view;

    [Header("Events")]
    [SerializeField] private GameEvent gameClearEvent;
    [Tooltip("Raise되면 SO 데이터 및 GameManager 상태가 초기화됨.")]
    [SerializeField] private GameEvent gameResetEvent;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName;

    private void Awake()
    {
        bool valid = true;

        if (!view)
        {
            Debug.LogError("[GameClearController] view is not assigned", this);
            valid = false;
        }
        if (!gameClearEvent)
        {
            Debug.LogError("[GameClearController] gameClearEvent is not assigned ? game clear UI will never activate", this);
            valid = false;
        }
        if (!gameResetEvent)
        {
            Debug.LogError("[GameClearController] gameResetEvent is not assigned ? GameManager will remain paused after restart/main menu", this);
            valid = false;
        }
        if (!scoreSO)
        {
            Debug.LogError("[GameClearController] scoreSO is not assigned ? final score will always display 0", this);
            valid = false;
        }

        if (!valid) enabled = false;
    }

    private void OnEnable()
    {
        if (!view) return;

        if (gameClearEvent) gameClearEvent.RegisterListener(HandleGameOver);

        view.OnRestartClicked.AddListener(HandleRestart);
        view.OnMainMenuClicked.AddListener(HandleMainMenu);
        view.OnQuitClicked.AddListener(HandleQuit);
    }

    private void OnDisable()
    {
        if (!view) return;

        if (gameClearEvent) gameClearEvent.UnregisterListener(HandleGameOver);

        view.OnRestartClicked.RemoveListener(HandleRestart);
        view.OnMainMenuClicked.RemoveListener(HandleMainMenu);
        view.OnQuitClicked.RemoveListener(HandleQuit);
    }

    private void HandleGameOver() //HandleGameClear로
    {
        HandleGameClear();
    }

    private void HandleGameClear()
    {
        int finalScore = scoreSO ? scoreSO.Value : 0;
        bool isNewRecord = highScoreSO != null && highScoreSO.TrySave(finalScore);
        int highScore = highScoreSO != null ? highScoreSO.Value : finalScore;
        view.Show(finalScore, highScore, isNewRecord);
        GameManager.Instance?.NotifyGameClear();
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
