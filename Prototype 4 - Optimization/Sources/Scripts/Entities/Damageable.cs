using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Defines a damageable entity
/// </summary>
[RequireComponent(typeof(EntityIdentity))]
public class Damageable : MonoBehaviour, IDamageProcessor
{
	protected EntityIdentity _identity;

	public bool IsDead => _identity.CurrentHealth <= 0;

	public static event Action<EntityIdentity> OnDeath;
	public event Action OnDamage;

	protected virtual void Awake()
	{
		_identity = GetComponent<EntityIdentity>();
	}

	public virtual void ApplyDamage(GameObject attacker, float damage)
	{
		if (IsDead) return;

		_identity.CurrentHealth -= damage;
		OnDamage?.Invoke();
		if (IsDead)
			Kill(attacker);
	}

	public virtual void Kill(GameObject attacker)
	{
		OnDeath?.Invoke(_identity);
	}
}
