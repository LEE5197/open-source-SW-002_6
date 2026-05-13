using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Vector2 moveVec = Vector2.zero;
    private bool canFire = true;
    private bool isFire = false;

    public GameObject defaultBulletPrefab;
    public float moveSpeed = 8f;
    public float fireDelay = 0.2f;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (isFire && canFire)
        {
            Fire();
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rigid.linearVelocity = moveVec * moveSpeed;
    }
    private void Fire()
    {
        Instantiate(defaultBulletPrefab, transform.position, transform.rotation);
        StartCoroutine(FireCoroutine());
    }
    IEnumerator FireCoroutine()
    {
        canFire = false;
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
    }
    void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>();
    }
    void OnFire(InputValue value)
    {
        isFire = value.isPressed;
    }
}
