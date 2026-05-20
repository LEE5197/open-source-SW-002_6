using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //ΩÃ±€≈Ê ∆–≈œ
    public bool IsGameRunning = true;
    public bool IsPaused { get; private set; }

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
