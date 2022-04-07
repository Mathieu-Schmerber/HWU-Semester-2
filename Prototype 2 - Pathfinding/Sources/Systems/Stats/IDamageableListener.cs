using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageableListener
{
	void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount);
	void OnDeath(GameObject attacker, IDamageProcessor victim);
}