using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private HighScoreSO highScoreSO;

    [Header("View")]
    [SerializeField] private StartMenuView view;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName;

    private void Awake()
    {
        if (!view)
        {
            Debug.LogError("[StartMenuController] view is not assigned", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!view) return;

        view.OnStartClicked.AddListener(HandleStart);
        view.OnQuitClicked.AddListener(HandleQuit);

        int highScore = highScoreSO != null ? highScoreSO.Value : 0;
        view.SetHighScore(highScore);
    }

    private void OnDisable()
    {
        if (!view) return;

        view.OnStartClicked.RemoveListener(HandleStart);
        view.OnQuitClicked.RemoveListener(HandleQuit);
    }

    private void HandleStart()
    {
        if (string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(gameSceneName);
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
