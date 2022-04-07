using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Nawlian.Lib.Systems.Pooling;

/// <summary>
/// Plays selected animations OnDamage and OnDeath
/// </summary>
public class EnemyDamageable : Damageable
{
	#region Fields

	[SerializeField] private float _destroyDelay;

	private TrailRenderer _trailRenderer;
	private bool _visible;
	private IPoolableObject _poolId;

	#endregion

	protected override void Awake()
	{
		base.Awake();
		_trailRenderer = GetComponentInChildren<TrailRenderer>();
		_poolId = GetComponent<IPoolableObject>();
	}

	private void OnBecameVisible() => _visible = true;

	private void OnBecameInvisible() => _visible = false;

	private void OnEnable()
	{
		GameLoop.OnGameEnded += Release;
	}

	private void OnDisable()
	{
		GameLoop.OnGameEnded -= Release;
	}

	public override void Kill(GameObject attacker)
	{
		base.Kill(attacker);
		_trailRenderer.Clear();

		// Appy effects only if the enemy was visible
		if (_visible)
		{
			if (attacker.layer.Equals(GameLoop.Player.gameObject.layer))
				GameLoop.Camera.Shake(Vector3.one * .2f, .1f);
			Blast.Spawn(transform.position, Blast.Size.MEDIUM);
			GoldGainText.Spawn(transform.position, _identity.CoinWorth);
		}
		Invoke(nameof(Release), _destroyDelay);
	}

	private void Release() => _poolId.Release();
}