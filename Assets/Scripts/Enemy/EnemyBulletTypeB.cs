using UnityEngine;

public class EnemyBulletTypeB : MonoBehaviour
{
    private Rigidbody2D rigid;

    public Vector2 moveVec = Vector2.zero;
    public float moveSpeed = 5f;
    public float damage = 5f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rigid.linearVelocity = moveVec * moveSpeed;
    }

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
