using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get; private set; }

    [Header("Events")]
    [Tooltip("Â Â?€ê¹?ÂŠë©?? RaiseÂÂ˜ï§? å¯ƒÂ?ÂÂ„ ÂƒÂÂƒÂœåª›Â€ ?¥Âˆæ¹²ê³ Â™Â?Â(IsGameRunning=true, Resume).")]
    [SerializeField] private GameEvent resetEvent;
    public bool IsGameRunning = true;
    public Transform playerTransform;
    //?±ê????¨í„´

    private float cachedTimeScale = 1f;

    //?¤ë¸Œ?íŠ¸ ???ìš©???„ë¦¬??
    [Header("ObjectPool")]
    public GameObject PlayerBulletPrefab;
    public GameObject EnemyBulletPrefab;

    // ?¤ë¸Œ?íŠ¸ ??
    private Queue<PlayerBullet> playerBullets;
    private Queue<EnemyBullet> enemyBullets;

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

        if (playerTransform == null)
            playerTransform = GameObject.FindWithTag("Player").transform;

        playerBullets = new Queue<PlayerBullet>();
        enemyBullets = new Queue<EnemyBullet>();

        AddPlayerBullet();
        AddEnemyBullet();
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
    
    public void NotifyGameClear() //°ÔÀÓ Å¬¸®¾î½Ã
    {
        IsGameRunning = false;
        Pause();
    }

    //?Œë ˆ?´ì–´ ì´ì•Œ ë¦¬ìŠ¤?¸ì— ì´ì•Œ ì¶”ê?
    private void AddPlayerBullet()
	{
        for(int i = 0; i < 30; i++)
		{
            GameObject bullet = Instantiate(PlayerBulletPrefab, transform.position, Quaternion.identity, null);
            bullet.SetActive(false);

            playerBullets.Enqueue(bullet.GetComponent<PlayerBullet>());
		}
	}

    //?Œë ˆ?´ì–´ ì´ì•Œ??ë°œì‚¬???¤ë¸Œ?íŠ¸?ì„œ ì´ì•Œ ? ë‹¹???”ì²­?????ˆëŠ” ?¨ìˆ˜
    public PlayerBullet GetPlayerBullet()
	{
		if (playerBullets.Count == 0)
		{
            AddPlayerBullet();
		}

        return playerBullets.Dequeue();
	}
    //ì´ì•Œ???¬ë¬¼ê³?ì¶©ëŒ?ˆì„ ?? ?¤ì‹œ ObjectPool???£ëŠ” ?¨ìˆ˜
    public void ReturnPlayerBullet(PlayerBullet bullet)
	{
        bullet.gameObject.SetActive(false);
        playerBullets.Enqueue(bullet);
	}

    //??ì´ì•Œ ë¦¬ìŠ¤?¸ì— ì´ì•Œ ì¶”ê?
    private void AddEnemyBullet()
    {
        for(int i = 0; i < 30; i++)
		{
            GameObject bullet = Instantiate(EnemyBulletPrefab, transform.position, Quaternion.identity, null);
            bullet.SetActive(false);

            enemyBullets.Enqueue(bullet.GetComponent<EnemyBullet>());
        }
        //?¤ë¸Œ?íŠ¸ ??ì²´í¬??ë¡œê·¸
        //Debug.Log($"Add enemy bullets, current total : {enemyBullets.Count}");
    }

    //??ì´ì•Œ??ë°œì‚¬???¤ë¸Œ?íŠ¸?ì„œ ì´ì•Œ ? ë‹¹???”ì²­?????ˆëŠ” ?¨ìˆ˜
    public EnemyBullet GetEnemyBullet()
    {
        if (enemyBullets.Count == 0)
        {
            AddEnemyBullet();
        }

        return enemyBullets.Dequeue();
    }
    //ì´ì•Œ???¬ë¬¼ê³?ì¶©ëŒ?ˆì„ ?? ?¤ì‹œ ObjectPool???£ëŠ” ?¨ìˆ˜
    public void ReturnEnemyBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        enemyBullets.Enqueue(bullet);
    }
}
