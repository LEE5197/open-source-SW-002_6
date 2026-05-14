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

    // 총알의 충돌을 감지하기위한 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 발사한 총알은 플레이어를 무시하도록 설정
        if (collision.gameObject.tag=="Player") return;

        // 오브젝트 폴링 기법 사용을 위한 충돌 시 오브젝트 비활성화 설정
        this.gameObject.SetActive(false);
    }
}
