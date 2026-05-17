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

	// 공격에 필요한 총알 프리펩, 리스트 등 변수
	public GameObject bulletPrefab;
	public float attackDelay = 1f;
	public LayerMask enemyLayer;
	public float attackRange = 7f;

	private bool canAttack = true;
	private List<GameObject> bulletObjList;
	private List<PlayerBullet> bulletVecList;
	int idx = 0;

	private void Awake()
	{
        rigid = GetComponent<Rigidbody2D>();
		bulletObjList = new List<GameObject>();
		bulletVecList = new List<PlayerBullet>();

		for(int i = 0; i < 10; i++)
		{
			GameObject obj = Instantiate(bulletPrefab);
			bulletObjList.Add(obj);
			bulletVecList.Add(obj.GetComponent<PlayerBullet>());
			obj.SetActive(false);
		}
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

	private void Attack()
	{
		Collider2D enemy = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
		if (enemy == null) return;

		canAttack = false;
		StartCoroutine(AttackCoroutine());
		bulletVecList[idx].moveVec = (enemy.gameObject.transform.position - transform.position).normalized;
		bulletObjList[idx].transform.position = transform.position;
		bulletObjList[idx].SetActive(true);

		idx = (idx + 1) % bulletObjList.Count;
	}
	IEnumerator AttackCoroutine()
	{
		yield return new WaitForSeconds(attackDelay);
		canAttack = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
