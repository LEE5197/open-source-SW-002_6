using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector2 moveVec = Vector2.down;

    private Rigidbody2D rigid;
    public float moveSpeed = 2f;
    private Vector2 bulletVec;
    public float damage = 1f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void OnEnable() //활성화 시에 방향 결정 => 오브젝트 폴 및 보스 오브젝트에서도 동일한 총알 사용을 위해 수정, 적 오브젝트에서 총알 발사시 총알 위치 지정하도록 변경
    {
        /*
        if (playerTransform != null)
            bulletVec = (playerTransform.position - transform.position).normalized;
        */
    }
    private void OnDisable()
    {
        rigid.linearVelocity = Vector2.zero; //속도 초기화
    }

    //방향으로 이동
    private void FixedUpdate()
    {
        rigid.linearVelocity = moveVec * moveSpeed;
    }

    // 총알의 충돌을 감지하기위한 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy") return; //적끼리는 무시

        if (collision.gameObject.layer == 6) //Player와 만나면 제거
        {
            GameManager.Instance.ReturnEnemyBullet(this);
        }

        if (collision.gameObject.layer == 0) //Border와 만나면 제거
            GameManager.Instance.ReturnEnemyBullet(this);

        if (collision.gameObject.CompareTag("Ult")) // Ult와 충돌 시 제거
            GameManager.Instance.ReturnEnemyBullet(this);
    }
}
