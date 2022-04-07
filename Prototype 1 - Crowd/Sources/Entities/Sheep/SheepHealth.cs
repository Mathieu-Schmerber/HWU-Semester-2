using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepHealth : ADamageable
{
	[SerializeField] private GameObject _deathFX;

	protected override void OnDeath()
	{
		Spawner.Instance.KillSheep(GetComponent<SheepAI>());
	}

	private void OnDestroy()
	{
		Instantiate(_deathFX, transform.position, Quaternion.identity);
	}
}
