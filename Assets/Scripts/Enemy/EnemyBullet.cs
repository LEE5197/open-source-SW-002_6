using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Transform playerTransform; //player 오브젝트

    private Rigidbody2D rigid;
    public float moveSpeed = 2f;
    private Vector2 bulletVec;
    public float damage = 1f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (playerTransform == null) //플레이어 위치 추가
            playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public void OnEnable() //활성화 시에 방향 결정
    {
        if (playerTransform != null)
            bulletVec = (playerTransform.position - transform.position).normalized;
    }
    private void OnDisable()
    {
        rigid.linearVelocity = Vector2.zero; //속도 초기화
    }

    //방향으로 이동
    private void FixedUpdate()
    {
        rigid.linearVelocity = bulletVec * moveSpeed;
    }

    // 총알의 충돌을 감지하기위한 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy") return; //적끼리는 무시

        if (collision.gameObject.layer == 6) //Player와 만나면 제거
        {
            this.gameObject.SetActive(false);
        }

        if (collision.gameObject.layer == 0) //Border와 만나면 제거
            this.gameObject.SetActive(false);

        if (collision.gameObject.CompareTag("Ult")) // Ult와 충돌 시 제거
            this.gameObject.SetActive(false);
    }
}
