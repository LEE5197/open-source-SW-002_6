using UnityEngine;
using System.Collections;

public class PlayerUlt : MonoBehaviour
{
    private Rigidbody2D rigid;
	public float moveSpeed = 20f;

    private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
    }
	private void OnEnable()
	{
		//Debug.Log("active ult");
		moveSpeed = 10f;
		transform.localScale = new Vector3(1f, 1f, 1f);
		StartCoroutine(ScaleRoutine());
	}

	private void FixedUpdate()
	{
		rigid.linearVelocity = Vector2.up * moveSpeed;
	}

	IEnumerator ScaleRoutine()
	{
		yield return new WaitForSeconds(0.5f);
		transform.localScale = new Vector3(10f, 10f, 10f);
		moveSpeed = 5f;
	}
}
