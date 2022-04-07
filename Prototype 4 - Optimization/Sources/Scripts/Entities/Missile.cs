using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
	private Transform _target;

	public override void Init(ProjectilePool.InitData data)
	{
		base.Init(data);
		_trail.enabled = true;
		Invoke(nameof(FindTarget), 0.1f);
	}

	private void FindTarget()
	{
		Collider2D around = Physics2D.OverlapCircle(transform.position, 15, _constants.EnemyLayer);

		if (around != null)
			_target = around.transform;
	}

	private void Update()
	{
		if (_target != null)
		{
			if (_target.gameObject.activeSelf == false)
			{
				FindTarget();
				return;
			}
			transform.up = Vector3.Slerp(transform.up, (_target.position - transform.position).normalized, 0.2f);
			transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
			_rb.velocity = transform.up * _data.BaseSpeed;
		}
	}

	public override void OnLifetimeEnd()
	{
		Collider2D[] around = Physics2D.OverlapCircleAll(transform.position, 2f);

		foreach (Collider2D collider in around)
		{
			if (collider.gameObject.layer == _shooter.gameObject.layer)
				continue;
			IDamageProcessor processor = collider.gameObject.GetComponent<IDamageProcessor>();
			processor?.ApplyDamage(_shooter.gameObject, (int)(_data.BaseDamage * _shooter.DamageMultiplier.Value));
		}

		// FX
		Blast.Spawn(transform.position, Blast.Size.BIG); CancelInvoke(nameof(FindTarget));
	}
}