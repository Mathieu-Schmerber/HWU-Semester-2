using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Automatically destroys this gameobject after every particle system has been played.
/// </summary>
public class AutoDestroyParticle : MonoBehaviour
{
	private List<ParticleSystem> _ps;
	private void Awake()
	{
		_ps = GetComponentsInChildren<ParticleSystem>().ToList();
		_ps.Add(GetComponent<ParticleSystem>());
	}

	private void Start()
	{
		float time = _ps.Max(x => x.main.startLifetime.constantMax);

		Destroy(gameObject, time);
	}
}