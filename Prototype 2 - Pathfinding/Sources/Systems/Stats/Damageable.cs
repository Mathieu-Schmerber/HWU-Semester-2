using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Defines a damageable entity
/// </summary>
public class Damageable : MonoBehaviour, IDamageProcessor
{
    private EntityIdentity _stats;
	private List<IDamageableListener> _damageableListeners;

	public bool IsDead => _stats.Health.Value <= 0 && _stats.Health.Enabled;

	private void Awake()
	{
		_stats = GetComponent<EntityIdentity>();
		_damageableListeners = GetComponentsInChildren<IDamageableListener>().ToList();
	}

	private void Start()
	{
		if (!_stats.Health.Enabled)
			Debug.LogError($"{gameObject.name} has an ADamageable component while not having Stats.Health enabled.");
	}

	public void ApplyDamage(GameObject attacker, int damage)
	{
		if (IsDead) return;

		_stats.Health.Value -= damage;
		_damageableListeners.ForEach(x => x.OnDamageDealt(attacker, this, damage));
		if (IsDead)
			Kill(attacker);
	}

	public void Kill(GameObject attacker)
	{
		_stats.Health.Value = 0;
		_damageableListeners.ForEach(x => x.OnDeath(attacker, this));
	}
}
