using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageProcessor
{
	bool IsDead { get; }

	void ApplyDamage(GameObject attacker, int amount);
}