using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer render;
    [SerializeField] private ScoreSO scoreSO;
    [SerializeField] private Slider hpBar;

    public Transform playerTransform;
    public GameObject bulletPrefab;
    public GameObject bulletPrefabTypeB;

    public bool isRage = false;         //최대 체력이 50% 미만인지 확인
    public float maxHealth = 10000f;    //최대 체력
    private float Health = 10000f;      //현재 체력
    public float moveSpeed = 1.0f;      //이동 속도
    public float minDistX = 0.2f;       //최소 x축 거리
    public float minDistY = 6f;         //최소 y축 거리

    public int score = 1000;  //처치 시 얻는 점수

    public float fireDelay = 2f; //공격 속도
    public float fireGap = 0.1f; //탄환 발사 간격

    //일정 횟수 플레이어 방향으로 연속해서 발사
    #region Fire Forward
    public int bulletNum = 10; //연속으로 발사하는 총알 숫자 
    #endregion

    //일정 탄환을 부채꼴 모양으로 한번에 발사
    #region Fire Shot
    public int fireShotBulletNum = 30;
    public float angle = 45f;
    #endregion

    //일정 횟수 플레이어 방향 기반으로 무작위 방향 발사
    #region Fire Rapid
    public int fireRapidBulletNum = 90;
    #endregion
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        Health = maxHealth;
        hpBar.value = Health / maxHealth;

        StartCoroutine(FireForward());
        StartCoroutine(FireShot());
        StartCoroutine(FireRapid());
    }

    private void Update()
    {
        hpBar.value = Health / maxHealth;
    }

    private void FixedUpdate()
    {
        Move();
    }

    //플레이어 방향 기준 여러발 부채꼴 모양으로 발사
    IEnumerator FireShot()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireDelay + 0.1f);

            if (playerTransform == null) continue;

            //보스에서 플레이어를 바라보는 기본 방향 벡터와 중심 각도
            Vector2 dir = playerTransform.position - transform.position;
            float centerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //부채꼴의 시작 각도
            float startAngle = centerAngle - (angle / 2f);

            //30발의 총알이 부채꼴 공간에 고르게 퍼지기 위한 간격 각도를 계산합니다.
            float angleStep = angle / (fireShotBulletNum - 1);

            //지정한 총알 개수 한 발씩 각도를 계산해 발사
            for (int i = 0; i < fireShotBulletNum; i++)
            {
                // 이번 차례에 쏠 총알의 최종 각도
                float targetAngle = startAngle + (angleStep * i);

                //라디안으로 변환하여 2D 방향 벡터로 변경
                float rad = targetAngle * Mathf.Deg2Rad;
                Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                //총알을 생성
                GameObject bullet = Instantiate(bulletPrefabTypeB, transform.position, Quaternion.identity);

                //생성된 총알에 계산해 둔 방향 벡터 설정
                EnemyBulletTypeB bulletScript = bullet.GetComponent<EnemyBulletTypeB>();
                if (bulletScript != null)
                {
                    bulletScript.moveVec = fireDirection;
                }

                // 총알 오브젝트의 이미지 각도도 날아가는 방향을 바라보게 회전시켜 줍니다.
                bullet.transform.rotation = Quaternion.Euler(0, 0, targetAngle - 90f);
            }
        }
    }
    // 플레이어 방향으로 여러발 일정 간격으로 발사
    IEnumerator FireForward()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireDelay);

            for (int i = 0; i < bulletNum; i++)
            {
                yield return new WaitForSeconds(fireGap);
                Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
            }
        }
    }
    IEnumerator FireRapid()
    {
        while (true)
        {
            // 원하는 공격 딜레이만큼 대기
            yield return new WaitForSeconds(fireDelay);

            if (playerTransform == null) continue;

            for (int i = 0; i < fireRapidBulletNum; i++)
            {
                // 연사 느낌을 주기 위해 총알 한 발당 미세한 시간 차이를 둡니다. (원치 않으면 삭제 가능)
                yield return new WaitForSeconds(fireGap);
                if (playerTransform == null) break;

                // 1. 보스에서 플레이어를 바라보는 기본 방향과 중심 각도를 구합니다.
                Vector2 dir = playerTransform.position - transform.position;
                float centerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                // 2. [핵심] 중심 각도에서 지정한 범위(45도)의 절반만큼 좌우로 무작위 오차를 줍니다.
                // 예: 45도 설정 시, -22.5도 ~ +22.5도 사이의 랜덤한 값이 더해집니다.
                float randomOffset = Random.Range(-angle / 2f, angle / 2f);
                float finalAngle = centerAngle + randomOffset;

                // 3. 최종 결정된 랜덤 각도를 2D 방향 벡터로 변환합니다.
                float rad = finalAngle * Mathf.Deg2Rad;
                Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                // 4. 총알을 생성하고 방향을 주입합니다.
                GameObject bullet = Instantiate(bulletPrefabTypeB, transform.position, Quaternion.identity);

                EnemyBulletTypeB bulletScript = bullet.GetComponent<EnemyBulletTypeB>();
                if (bulletScript != null)
                {
                    bulletScript.moveVec = fireDirection;
                }

                // 총알이 날아가는 방향을 바라보도록 회전
                bullet.transform.rotation = Quaternion.Euler(0, 0, finalAngle - 90f);
            }
        }
    }

    // 플레이어를 향해 이동
    private void Move()
    {
        if (playerTransform == null)
        {
            Debug.Log("Input player object");
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        // 플레이어와 보스의 실제 위치 차이 
        // 보스에서 플레이어로 향하는 방향 벡터
        float directionX = playerTransform.position.x - transform.position.x;
        float directionY = playerTransform.position.y - transform.position.y;

        float nextVelocityX = 0f;
        float nextVelocityY = 0f;

        // 플레이어와의 가로 거리가 가깝지 않다면 
        // moveSpeed만큼 이동
        if (Mathf.Abs(directionX) > minDistX)
        {
            nextVelocityX = Mathf.Sign(directionX) * moveSpeed;
        }

        // 보스가 화면 밖에서 등장해 6f 만큼 이동
        // 현재 내 y 좌표가 6f보다 크다면 아래로 이동

        if (transform.position.y > minDistY)
        {
            nextVelocityY = -moveSpeed; // 아래로 하강
        }
        else
        {
            nextVelocityY = 0f; // 원하는 높이에 도달하면 Y축 이동 중지
        }

        // 최종적으로 계산된 속도를 바탕으로 실질적인 이동
        rigid.linearVelocity = new Vector2(nextVelocityX, nextVelocityY);
    }
    IEnumerator HitEffect()
    {
        render.enabled = false;
        yield return new WaitForSeconds(0.05f);
        render.enabled = true;
    }
    private void Death()
    {
        scoreSO.AddScore(score);
        gameObject.SetActive(false);
    }
    private void Rage()
    {
        fireDelay = fireDelay / 2;
        fireGap = fireGap / 2;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) return;

        if (collision.gameObject.CompareTag("Ult"))
            Health -= 500;
        else
            Health -= collision.gameObject.GetComponent<PlayerBullet>().damage;
    

        if (Health <= 0f)
        {
            Death();
            return;
        }

        if (!isRage&&Health/maxHealth<=0.5f)
        {
            Debug.Log("in rage mode");
            Rage();
        }
        StartCoroutine(HitEffect());
    }
}
