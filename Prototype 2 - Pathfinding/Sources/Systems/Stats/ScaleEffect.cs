using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shakes the object's scale OnDamage and OnDeath
/// </summary>
public class ScaleEffect : MonoBehaviour, IDamageableListener
{
	[SerializeField] private Vector3 _intensity = new Vector3(0.5f, 0.5f, 0.5f);
	[SerializeField] private float _duration = 0.1f;
	[SerializeField] private bool _onDeath;
	[SerializeField] private bool _onSpawn;

	private void Start()
	{
		if (_onSpawn)
			iTween.ShakeScale(gameObject.transform.GetChild(0).gameObject, _intensity, _duration);
	}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount)
	{
		if (victim.IsDead && !_onDeath) return;

		iTween.ShakeScale(gameObject.transform.GetChild(0).gameObject, _intensity, _duration);
	}

	public void OnDeath(GameObject attacker, IDamageProcessor victim) {}
}
