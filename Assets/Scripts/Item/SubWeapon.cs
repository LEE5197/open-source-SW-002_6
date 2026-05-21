using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubWeapon : MonoBehaviour
{
	// 플레이어 위치 확인할 변수
    [SerializeField] private Transform targetPos;
	// 이동시키기 위한 변수
    private Rigidbody2D rigid;


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

		if (targetPos == null)
			targetPos = GameManager.Instance.playerTransform;
		if (targetPos = null)
			targetPos = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		if (canAttack)
		{
			Attack();
		}
	}
	private void FixedUpdate()
	{
		Move();
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
		if (enemy == null) return;

		canAttack = false;

		PlayerBullet bullet = GameManager.Instance.GetPlayerBullet();
		bullet.gameObject.SetActive(true);
		bullet.gameObject.transform.position = transform.position;
		bullet.moveVec = (enemy.gameObject.transform.position - transform.position).normalized;

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
