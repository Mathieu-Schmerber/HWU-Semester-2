using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Projectile
{
	public override void Init(ProjectilePool.InitData init)
	{
		base.Init(init);
		_rb.velocity = -transform.up * _data.BaseSpeed;
		_rb.drag = 2f;
	}

	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		if (enabled == false)
			return;
		else if (collision.gameObject.layer == _shooter.gameObject.layer)
			return;

		IDamageProcessor processor = collision.gameObject.GetComponent<IDamageProcessor>();

		if (processor != null)
			processor.ApplyDamage(_shooter.gameObject, (int)(_data.BaseDamage * _shooter.DamageMultiplier.Value));
	}

	private void Explode()
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
		Blast.Spawn(transform.position, Blast.Size.BIG);
	}

	public override void OnLifetimeEnd() => Explode();
}