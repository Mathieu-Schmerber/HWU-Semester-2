using UnityEngine;

public abstract class AEnemyController : MonoBehaviour
{
	protected Rigidbody2D _rb;
	protected EntityIdentity _target;
	protected EntityIdentity _identity;
	private Vector2 _movement;

	#region Unity builtins

	protected virtual void Awake()
	{
		_identity = GetComponent<EntityIdentity>();
		_rb = GetComponent<Rigidbody2D>();
		_target = GameLoop.Player;
	}

	protected virtual void Update()
	{
		if (!GameLoop.HasGameStarted) return;

		_movement = GetMovementNormal();
		transform.up = Vector3.Lerp(transform.up, _movement, 0.3f);
	}

	private void FixedUpdate() => Move();

	#endregion

	protected abstract Vector2 GetMovementNormal();

	protected virtual void Move()
	{
		if (!GameLoop.HasGameStarted) return;

		_rb.MovePosition(_rb.position + _movement * Time.fixedDeltaTime * _identity.Speed.Value);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject != _target.gameObject) return;

		_target.GetComponent<IDamageProcessor>()?.ApplyDamage(gameObject, _identity.CurrentHealth);
		GetComponent<IDamageProcessor>().ApplyDamage(gameObject, (int)(_identity.CurrentHealth));
	}
}