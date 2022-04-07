using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaWeapon : MonoBehaviour
{
	private Tesla _data;
	private ParticleSystem _ps;
	private Collider2D _collider;
	private AudioSource _source;

	private void Awake()
	{
		_source = GetComponent<AudioSource>();
		_ps = GetComponent<ParticleSystem>();
		_collider = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		GameLoop.OnGameStarted += GameLoop_OnGameStarted;
		GameLoop.OnGameEnded += GameLoop_OnGameEnded;
	}

	private void OnDisable()
	{
		GameLoop.OnGameStarted -= GameLoop_OnGameStarted;
		GameLoop.OnGameEnded -= GameLoop_OnGameEnded;
	}

	public void Init(Tesla tesla)
	{
		_data = tesla;
	}

	private void Attack()
	{
		_source.Play();
		_collider.enabled = true;
		_ps.Play(true);
		Tween.Value(1f, 0f, (value) => { }, 0.25f, 0, completeCallback: () => {
			_collider.enabled = false;
			_ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		});
	}

	private void GameLoop_OnGameStarted()
	{
		InvokeRepeating(nameof(Attack), 0, _data.AttackRate);
		transform.localScale = Vector3.one * _data.Range;
	}

	private void GameLoop_OnGameEnded()
	{
		CancelInvoke(nameof(Attack));
		_collider.enabled = false;
		_ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == transform.parent.gameObject) return;

		IDamageProcessor damageProcessor = collision.GetComponent<IDamageProcessor>();
		damageProcessor?.ApplyDamage(gameObject, _data.Damage);
	}
}
