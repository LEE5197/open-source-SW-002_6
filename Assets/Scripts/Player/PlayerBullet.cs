using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rigid;
    public float moveSpeed = 10f;
    public float damage = 1f;

    private void Awake()
    {
        // 총알 이동을 위해 Rigidbody2D 컴포넌트 rigid 변수에 초기화
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 총알 발사 이후 움직이도록 하는 코드
        rigid.linearVelocity = Vector2.up * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag=="Player") return;

        Destroy(gameObject);
    }
}
