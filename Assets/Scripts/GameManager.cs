using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameRunning = true;

    //ΩÃ±€≈Ê ∆–≈œ

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
