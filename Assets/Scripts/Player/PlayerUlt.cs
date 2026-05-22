using UnityEngine;

public class PlayerUlt : MonoBehaviour
{
    private Rigidbody2D rigid;
	public float moveSpeed = 20f;

    private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
    }

	private void FixedUpdate()
	{
		rigid.linearVelocity = Vector2.up * moveSpeed;
	}
}
