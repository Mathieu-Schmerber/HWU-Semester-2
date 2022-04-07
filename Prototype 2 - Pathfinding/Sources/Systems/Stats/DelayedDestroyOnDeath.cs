using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroyOnDeath : MonoBehaviour, IDamageableListener
{
	[SerializeField] private float _delay;

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount)
	{
	}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		Destroy(gameObject, _delay);
	}
}
