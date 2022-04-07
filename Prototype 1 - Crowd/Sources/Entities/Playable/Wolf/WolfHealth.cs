using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfHealth : ADamageable
{
	protected override void OnDeath() => Spawner.Instance.KillWolf(GetComponent<WolfEmitter>());
}
