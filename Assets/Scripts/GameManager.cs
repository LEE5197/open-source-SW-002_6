using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //싱글톤 패턴
    public bool IsGameRunning = true;
    public bool IsPaused { get; private set; }

    [Header("Events")]
    [Tooltip("이 이벤트가 Raise되면 게임 상태가 초기화됨(IsGameRunning=true, Resume).")]
    [SerializeField] private GameEvent resetEvent;

    private float cachedTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (resetEvent != null) resetEvent.RegisterListener(HandleReset);
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        if (resetEvent != null) resetEvent.UnregisterListener(HandleReset);
    }

    private void HandleReset()
    {
        IsGameRunning = true;
        Resume();
    }

    public void Pause()
    {
        if (IsPaused) return;
        cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsPaused) return;
        Time.timeScale = cachedTimeScale;
        IsPaused = false;
    }

    public void NotifyGameOver()
    {
        IsGameRunning = false;
        Pause();
    }
}
