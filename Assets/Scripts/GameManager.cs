using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool IsGameRunning = true;
    public Transform playerTransform;
    //싱글톤 패턴

    public static GameManager Instance;

    //오브젝트 폴 적용할 프리펩
    [Header("ObjectPool")]
    public GameObject PlayerBulletPrefab;
    public GameObject EnemyBulletPrefab;

    // 오브젝트 폴
    private Queue<PlayerBullet> playerBullets;
    private Queue<EnemyBullet> enemyBullets;
    
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

        if (playerTransform == null)
            playerTransform = GameObject.FindWithTag("Player").transform;

        playerBullets = new Queue<PlayerBullet>();
        enemyBullets = new Queue<EnemyBullet>();

        AddPlayerBullet();
        AddEnemyBullet();
    }

    //플레이어 총알 리스트에 총알 추가
    private void AddPlayerBullet()
	{
        for(int i = 0; i < 30; i++)
		{
            GameObject bullet = Instantiate(PlayerBulletPrefab, transform.position, Quaternion.identity, null);
            bullet.SetActive(false);

            playerBullets.Enqueue(bullet.GetComponent<PlayerBullet>());
		}
	}

    //플레이어 총알을 발사할 오브젝트에서 총알 할당을 요청할 수 있는 함수
    public PlayerBullet GetPlayerBullet()
	{
		if (playerBullets.Count == 0)
		{
            AddPlayerBullet();
		}

        return playerBullets.Dequeue();
	}
    //총알이 사물과 충돌했을 때, 다시 ObjectPool에 넣는 함수
    public void ReturnPlayerBullet(PlayerBullet bullet)
	{
        bullet.gameObject.SetActive(false);
        playerBullets.Enqueue(bullet);
	}

    //적 총알 리스트에 총알 추가
    private void AddEnemyBullet()
    {
        for(int i = 0; i < 30; i++)
		{
            GameObject bullet = Instantiate(EnemyBulletPrefab, transform.position, Quaternion.identity, null);
            bullet.SetActive(false);

            enemyBullets.Enqueue(bullet.GetComponent<EnemyBullet>());
        }
        //오브젝트 폴 체크용 로그
        //Debug.Log($"Add enemy bullets, current total : {enemyBullets.Count}");
    }

    //적 총알을 발사할 오브젝트에서 총알 할당을 요청할 수 있는 함수
    public EnemyBullet GetEnemyBullet()
    {
        if (enemyBullets.Count == 0)
        {
            AddEnemyBullet();
        }

        return enemyBullets.Dequeue();
    }
    //총알이 사물과 충돌했을 때, 다시 ObjectPool에 넣는 함수
    public void ReturnEnemyBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        enemyBullets.Enqueue(bullet);
    }
}
