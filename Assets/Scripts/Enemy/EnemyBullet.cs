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
        if (playerTransform  != null )
            bulletVec = (playerTransform.position - transform.position).normalized; //생성시에 방향 결정
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

        Destroy(this.gameObject);
    }
}
