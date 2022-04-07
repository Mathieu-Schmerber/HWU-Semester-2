using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Plays selected animations OnDamage and OnDeath
/// </summary>
public class DestroyOnDeath : MonoBehaviour, IDamageableListener
{
	#region Fields

	[SerializeField] private float _destroyDelay;

	#endregion
	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) {}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		Destroy(gameObject, _destroyDelay);
	}
}