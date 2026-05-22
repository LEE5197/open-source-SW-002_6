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

    [Header("Default setting")]
    public bool isRage = false;         //최대 체력이 50% 미만인지 확인
    public float maxHealth = 10000f;    //최대 체력
    private float Health = 10000f;      //현재 체력
    public float moveSpeed = 1.0f;      //이동 속도
    public float minDistX = 0.2f;       //최소 x축 거리
    public float minDistY = 6f;         //최소 y축 거리

    public int score = 1000;  //처치 시 얻는 점수

    //보스 공격에 사용할 공통 변수
    [Space]
    [Header("Default attack parameter")]
    public float fireDelay = 2f; //공격 속도
    public float fireGap = 0.1f; //탄환 발사 간격
    private bool canAttack = true;

    [Header("Audio Clips")]
    public AudioClip BossBGMClip;

    //일정 횟수 플레이어 방향으로 연속해서 발사
    [Space]
    [Header("Fire Forward")]
    #region Fire Forward
    public int bulletNum = 10; //연속으로 발사하는 총알 숫자 
    public int fireForwardCount = 5;    //패턴 수행 횟수
    #endregion

    //일정 탄환을 부채꼴 모양으로 한번에 발사
    [Header("Fire Circle Sector")]
    #region Fire Shot
    public int fireShotBulletNum = 30;
    public float angle = 45f;
    public int fireSectorCount = 5;      //패턴 수행 횟수
    #endregion

    //일정 횟수 플레이어 방향 기반으로 무작위 방향 발사
    [Header("Fire Spree")]
    #region Fire Rapid
    public int fireRapidBulletNum = 90;
    #endregion

    //원형으로 탄환 발사
    [Header("Fire Circle")]
    public int fireCircleBulletNum = 359;
    public float fireCircleDelay = 1f;
    public int fireAroundCount = 5;
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
    }

	private void Start()
	{
        playerTransform = GameManager.Instance.playerTransform;
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        SoundManager.Instance.PlayBossBGM(BossBGMClip);
    }

    private void Update()
    {
        hpBar.value = Health / maxHealth;
    }

    private void FixedUpdate()
    {
        Move();
        if (canAttack) Attack();
    }

    //공격 패턴을 정하고 작동
    private void Attack()
	{
        canAttack = false;
        int patternCnt = 3;
        int num = Random.Range(0, patternCnt);

		switch (num)
		{
            case 0:
                StartCoroutine(FireForward());
                break;
            case 1:
                StartCoroutine(FireShot());
                break;
            case 2:
                StartCoroutine(FireAround());
                break;
            case 3:
                break;
		}
	}
    //전방에 총알을 연속해서 부채꼴 모양으로 발사
    IEnumerator FireAround()
    {
        float angleOffset = 0;
        for (int cnt = 0; cnt < fireAroundCount; cnt++) 
        {
            angleOffset += 15f * Mathf.Deg2Rad;
            yield return new WaitForSeconds(fireCircleDelay);
            for (int i = 0; i < fireCircleBulletNum; i++)
            {
                EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
                if (bullet == null) continue;
                bullet.gameObject.SetActive(true);
                bullet.gameObject.transform.position = transform.position;

                float curAngle = Mathf.PI * 2 * i / fireCircleBulletNum + angleOffset;
                bullet.moveVec = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)).normalized;
            }
        }
        canAttack = true;
    }
    //플레이어 방향 기준 여러발 부채꼴 모양으로 발사
    IEnumerator FireShot()
    {
        for (int cnt = 0; cnt < fireSectorCount; cnt++)
        {
            yield return new WaitForSeconds(fireDelay);

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

                //총알을 폴에서 가져오기
                EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
                if (bullet == null) continue;

                //가져온 총알에 계산해 둔 방향 벡터 설정
                bullet.gameObject.SetActive(true);
                bullet.gameObject.transform.position = transform.position;
                bullet.moveVec = fireDirection;

                // 총알 오브젝트의 이미지 각도도 날아가는 방향을 바라보게 회전시켜 줍니다.
                bullet.transform.rotation = Quaternion.Euler(0, 0, targetAngle - 90f);
            }
        }
        canAttack = true;
    }
    // 플레이어 방향으로 여러발 일정 간격으로 발사
    IEnumerator FireForward()
    {
        for (int cnt = 0; cnt < fireForwardCount; cnt++) 
        {
            yield return new WaitForSeconds(fireDelay);

            for (int i = 0; i < bulletNum; i++)
            {
                yield return new WaitForSeconds(fireGap);
                
                EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
                if (bullet == null) continue;

                bullet.gameObject.SetActive(true);
                bullet.gameObject.transform.position = transform.position;
                bullet.moveVec = (playerTransform.transform.position - transform.position).normalized;
            }
        }
        canAttack = true;
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
                // 연사 느낌을 주기 위해 총알 한 발당 미세한 시간 차이
                yield return new WaitForSeconds(fireGap);
                if (playerTransform == null) break;

                //보스에서 플레이어를 바라보는 기본 방향과 중심 각도 계산
                Vector2 dir = playerTransform.position - transform.position;
                float centerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                //설정 각도에 따라 총알 퍼짐 무작위 설정
                float randomOffset = Random.Range(-angle / 2f, angle / 2f);
                float finalAngle = centerAngle + randomOffset;

                //최종 결정된 랜덤 각도를 방향 벡터로 변환
                float rad = finalAngle * Mathf.Deg2Rad;
                Vector2 fireDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                //총알을 생성하고 방향을 주입
                EnemyBullet bullet = GameManager.Instance.GetEnemyBullet();
                if (bullet == null) continue;

                bullet.gameObject.SetActive(true);
                bullet.gameObject.transform.position = transform.position;
                bullet.moveVec = fireDirection;
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
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    //체력이 절반 미만일 시 특수 패턴 진입
    private void Rage()
    {
        StopAllCoroutines();
        canAttack = true;
        isRage = true;
        fireDelay /= 2;
        fireGap /= 2;
        StartCoroutine(FireRapid());
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
