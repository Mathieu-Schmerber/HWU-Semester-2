using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns particles OnDamage and OnDeath
/// </summary>
public class ParticlesOnDamage : MonoBehaviour, IDamageableListener
{
	[SerializeField, LabelText("Spawn particles on hit")] private ToggleableValue<GameObject> _hitFx;
	[SerializeField, LabelText("Spawn particles on death")] private ToggleableValue<GameObject> _deathFx;

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount)
	{
		if (_hitFx.Enabled)
			Instantiate(_hitFx.Value, transform.position, Quaternion.identity);
	}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		if (_deathFx.Enabled)
			Instantiate(_deathFx.Value, transform.position, Quaternion.identity);
	}
}
