using UnityEngine;

public class PlayerUlt : MonoBehaviour
{
    private Rigidbody2D rigid;
	public float moveSpeed = 20f;

	[Header("Audio Clips")]
    public AudioClip PlayerUltClip;

    private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx(PlayerUltClip);
        }
    }

	private void FixedUpdate()
	{
		rigid.linearVelocity = Vector2.up * moveSpeed;
	}
}
