using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
	private Rigidbody2D rigid;
    public Vector2 moveVec = Vector2.zero;
    public float moveSpeed = 10f;
    public float deleteTimer = 5f;

    [Header("Audio Clips")]
    public AudioClip ItemClip;

	// 이동에 필요한 컴포넌트 할당
    private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	// 아이템이 무작위 방향으로 이동시키기 위해 변수 초기화
	private void OnEnable()
	{
		float x = Random.Range(-1f, 1f);
		float y = Random.Range(-1f, 1f);

		moveVec = new Vector2(x, y).normalized;

		// deleteTimer 이후 오브젝트 삭제를 위한 함수 호출
		StartCoroutine(deleteObject());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void FixedUpdate()
	{
		// 아이템 오브젝트 이동
		rigid.linearVelocity = moveVec * moveSpeed;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 플레이어와 충돌하면 아이템 오브젝트 비활성화
		if (collision.gameObject.layer == 6)
		{
			//Debug.Log("hit player");
			if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySfx(ItemClip);
            }
			gameObject.SetActive(false);
			return;
		}
		
		// 경계선과 충돌하면 충돌 반대 방향으로 방향 전환
		switch (collision.gameObject.name)
		{
			case "Border_UP":
				moveVec.y = -moveVec.y;
				break;
			case "Border_DOWN":
				moveVec.y = -moveVec.y;
				break;
			case "Border_LEFT":
				moveVec.x = -moveVec.x;
				break;
			case "Border_RIGHT":
				moveVec.x = -moveVec.x;
				break;
		}
		
	}

	// 아이템 오브젝트의 최대 활성화 시간이 지날 시 비활성화
	IEnumerator deleteObject()
	{
		yield return new WaitForSeconds(deleteTimer);
		gameObject.SetActive(false);
	}
}
