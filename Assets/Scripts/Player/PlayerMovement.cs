using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 플레이어 오브젝트의 이동 속도
    [SerializeField] private float moveSpeed = 5f;
	// 플레이어 오브젝트를 움직이기 위한 Rigidbody2D 컴포넌트
	private Rigidbody2D rigid;
    // 플레이어의 이동 방향을 입력받기 위한 변수
    private Vector2 moveVec = Vector2.zero;

	private void Awake()
	{
		// Rigidbody2D 컴포넌트 rigid 변수에 추가
		rigid = GetComponent<Rigidbody2D>();
	}
	void Update()
    {

    }

	private void FixedUpdate()
	{
		Move();
	}

	//플레이어 오브젝트를 움직이는 함수
	private void Move()
	{
		rigid.linearVelocity = moveVec * moveSpeed;
	}

	// 유니티 input system 을 통해 플레이어의 이동 방향을 받기 위한 함수 호출
	private void OnMove(InputValue value)
	{
        moveVec = value.Get<Vector2>();
	}
}
