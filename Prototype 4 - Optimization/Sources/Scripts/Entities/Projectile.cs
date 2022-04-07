using Nawlian.Lib.Systems.Pooling;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
	protected ConstantData _constants;

	protected ProjectileData _data;
	protected Rigidbody2D _rb;
	protected BoxCollider2D _collider;
	protected SpriteRenderer _spriteRenderer;
	protected EntityIdentity _shooter;
	protected TrailRenderer _trail;
	protected IPoolableObject _poolId;

	protected virtual void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_collider = GetComponent<BoxCollider2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_constants = Resources.Load<ConstantData>("Data/Constants");
		_trail = GetComponent<TrailRenderer>();
		_poolId = GetComponent<IPoolableObject>();
	}

	public virtual void Init(ProjectilePool.InitData init)
	{
		_shooter = init.shooter;
		_data = init.data;
		transform.localScale = init.data.Scale;
		_spriteRenderer.sprite = _data.Graphics;
		_spriteRenderer.color = init.shooter.IsPlayer == true ? _constants.PlayerColor : _constants.EnemyColor;
		_rb.velocity = transform.up * _data.BaseSpeed;
		_rb.drag = 0f;
		_trail.Clear();
		_trail.enabled = false;
	}

	public virtual void OnLifetimeEnd() { }

	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		if (enabled == false)
			return;
		else if (collision.gameObject.layer == _shooter.gameObject.layer)
			return;

		IDamageProcessor processor = collision.gameObject.GetComponent<IDamageProcessor>();

		if (processor != null)
		{
			processor.ApplyDamage(_shooter.gameObject, (int)(_data.BaseDamage * _shooter.DamageMultiplier.Value));
			Blast.Spawn(transform.position, Blast.Size.SMALL);
			_poolId.Release();
		}
	}
}