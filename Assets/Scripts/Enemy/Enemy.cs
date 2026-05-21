using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    public GameObject enemyBullet; //적 bullet 프리팹
    public float moveSpeed = 3.0f;
    public float fireDelay = 1f; //공격 속도
    public int bulletNum = 1; //연속으로 발사하는 총알 숫자 
    public int score = 10;  //처치 시 얻는 점수

    private List<GameObject> EnemyBulletList;
    private int curIdx = 0;

    // 스크립터블 오브젝트를 통한 UI 연동
    [SerializeField] private ScoreSO scoreSO;

    [Header("Audio Clips")]
    public AudioClip enemyShootClip;
    public AudioClip enemyExplosionClip;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(Fire(bulletNum)); //발사 시작

        //풀링용 리스트
        EnemyBulletList = new List<GameObject>();
        for (int i = 0; i < 50; i++)
        {
            GameObject bullet = Instantiate(enemyBullet);
            bullet.SetActive(false);
            EnemyBulletList.Add(bullet);
        }
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
                EnemyBulletList[curIdx].transform.position = transform.position; //총알 생성
                EnemyBulletList[curIdx++].SetActive(true);
                curIdx %= EnemyBulletList.Count;
            }
        }
    }

    IEnumerator HitEffect()
    {
        moveSpeed = 0;

        SoundManager.Instance.PlaySfx(enemyExplosionClip);

        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        scoreSO.AddScore(score);
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
