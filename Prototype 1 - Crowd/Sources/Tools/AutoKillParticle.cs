using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKillParticle : MonoBehaviour
{
	private void Start()
	{
		ParticleSystem ps = GetComponent<ParticleSystem>();
		float lifetime = ps.main.startLifetime.constantMax;

		Invoke(nameof(KillParticle), lifetime);
	}

	private void KillParticle()
	{
		Destroy(gameObject);
	}
}
