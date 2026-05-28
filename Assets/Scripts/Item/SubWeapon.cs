using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class SubWeapon : MonoBehaviour
{
	// 플레이어 위치 확인할 변수
    [SerializeField] private Transform targetPos;
	// 이동시키기 위한 변수
    private Rigidbody2D rigid;
	// 바라보는 방향
	private Vector2 lookVec = Vector2.up;

	// 이동에 필요한 변수
	public Vector2 offset;
	public float maxSpeed = 8f;
    public float minDist = 1f;
    public float slowDist = 3f;

	// 공격 속도, 적 탐지 범위
	public float attackDelay = 1f;
	public LayerMask enemyLayer;
	public float attackRange = 7f;

	private bool canAttack = true;

	//이동에 필요한 Rigidbody2D 컴포넌트 할당, 플레이어 위치 참조
	private void Awake()
	{
        rigid = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		if (targetPos == null)
			targetPos = GameManager.Instance.playerTransform;
		if (targetPos == null)
			targetPos = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		if (canAttack)
		{
			Attack();
		}
		SetLookVec();
	}
	private void FixedUpdate()
	{
		Move();
	}
	// 적 방향을 향해 방향 전환
	private void SetLookVec()
	{
		// 가장 가까운 적 위치를 담을 변수
        Collider2D []enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
		Collider2D nearEnemy = null;

		// 가장 가까운 적을 알아내기 위한 변수
		float lowDist = Mathf.Infinity;

		foreach(Collider2D it in enemies)
		{	// 연산 부담을 줄이기 위해 sqrt 대신 sqrMagnitude 사용
			float dist = (transform.position - it.transform.position).sqrMagnitude;

			// 최소 거리가 현재 가리키는 적과의 거리보다 길다면 
			// 최소 거리와 현재 가리키는 적 위치 업데이트
			if (lowDist > dist)
			{
				nearEnemy = it;
				lowDist = dist;
			}
		}

		if (nearEnemy != null)
			lookVec = (nearEnemy.gameObject.transform.position - transform.position).normalized;
		else
			lookVec = Vector2.up;

        // 목표 방향 설정할 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, lookVec);

        // 일정한 속도로 회전
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            720f * Time.deltaTime
        );
    }
	// 플레이어 위치 고려해서 가까우면 느리게, 멀면 빠르게 이동
	private void Move()
	{

		Vector2 moveVec = (targetPos.position - transform.position) + (Vector3)offset;
		moveVec = moveVec.normalized;
		float dist = Vector2.Distance(targetPos.position, transform.position);

		if (dist < minDist)
		{
			rigid.linearVelocity = Vector2.zero;
			return;
		}

		float speedFactor = (dist - minDist) / (slowDist - minDist);
		speedFactor = Mathf.Clamp01(speedFactor);
		float currentSpeed = maxSpeed * speedFactor;

		rigid.linearVelocity = moveVec * currentSpeed;
	}

	//게임 매니저 오브젝트로부터 오브젝트 폴에 있는 총알 가져와서 발사
	private void Attack()
	{
		Collider2D enemy = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
		// 적이 없다면 바라보는 방향을 위로
		if (enemy == null)
		{
			lookVec = Vector2.up;
			return;
		}
		canAttack = false;

		// 총알 오브젝트 폴에서 가져와서 발사
		PlayerBullet bullet = GameManager.Instance.GetPlayerBullet();
		bullet.gameObject.SetActive(true);
		bullet.gameObject.transform.position = transform.position;
		bullet.moveVec = lookVec;
		bullet.transform.up = lookVec;

        StartCoroutine(AttackCoroutine());
	}
	//공격 속도 체크에 사용할 변수 초기화 
	IEnumerator AttackCoroutine()
	{
		yield return new WaitForSeconds(attackDelay);
		canAttack = true;
	}

	//디버깅 환경에서 공격 범위 보기위한 함수
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
