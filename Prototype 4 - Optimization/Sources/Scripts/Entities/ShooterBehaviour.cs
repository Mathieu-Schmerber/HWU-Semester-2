using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehaviour : AEnemyController
{
	private float _nextFire;
	[SerializeField] private float _fireRate;
	[SerializeField] private float _shootRange;
	[SerializeField] private ProjectileData _projectile;

	private Timer _shootCdr = new Timer();

	private void OnEnable() => _shootCdr.Start(_fireRate, true, Shoot);

	private void OnDisable() => _shootCdr.Stop();

	protected override Vector2 GetMovementNormal()
	{
		if (Vector3.Distance(transform.position, _target.transform.position) > _shootRange)
			return (_target.transform.position - transform.position).normalized;
		return Vector3.zero;
	}

	protected override void Update()
	{
		base.Update();
		transform.up = Vector3.Lerp(transform.up, (_target.transform.position - transform.position).normalized, 0.3f);
	}

	private void Shoot()
	{
		ProjectilePool.Spawn(
			position: transform.position + transform.up,
			rotation: transform.rotation,
			shooter: _identity,
			data: _projectile
		);
	}
}
