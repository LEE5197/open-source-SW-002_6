using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    public float moveSpeed = 3.0f;
    public float fireDelay = 1f; //공격 속도
    public int bulletNum = 1; //연속으로 발사하는 총알 숫자 
    public int score = 10;  //처치 시 얻는 점수


    // 스크립터블 오브젝트를 통한 UI 연동
    [SerializeField] private ScoreSO scoreSO;

    [Header("Audio Clips")]
    public AudioClip enemyShootClip;
    public AudioClip enemyExplosionClip;


    private Transform playerTransform;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        playerTransform = GameManager.Instance.playerTransform;
        if (playerTransform == null)
            playerTransform = GameObject.FindWithTag("Player").transform;

        StartCoroutine(Fire(bulletNum)); //발사 시작
    }
    private void FixedUpdate()
    {
        rigid.linearVelocity = transform.up * moveSpeed; //이동
    }

    IEnumerator Fire(int bulletNum)
    {
        while (true)
        {
            yield return new WaitForSeconds(fireDelay); //지연 시간만큼 대기

            for (int i = 0; i < bulletNum; i++) //총알 개수만큼 연속 발사
            {
                yield return new WaitForSeconds(0.1f);
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySfx(enemyShootClip);
                }

                EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
                if (bullet == null) continue;
                bullet.gameObject.SetActive(true);
                bullet.gameObject.transform.position = transform.position;
                bullet.moveVec = (playerTransform.position - transform.position).normalized;
            }
        }
    }

    IEnumerator HitEffect()
    {
        moveSpeed = 0;

        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        SoundManager.Instance.PlaySfx(enemyExplosionClip);

        scoreSO.AddScore(score);
        // 아이템 호출
        GameManager.Instance.GetItem(transform.position);
        Destroy(this.gameObject);
    }

    // 총알의 충돌을 감지하기위한 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy") return;

        if (collision.gameObject.layer == 8) //PlayerBullet과 충돌시
        {
            StartCoroutine(HitEffect());
        }

        if (collision.gameObject.layer == 0) //Border와 만나면 제거
            Destroy(this.gameObject);
    }
}
