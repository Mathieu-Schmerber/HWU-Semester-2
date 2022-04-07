using Nawlian.Lib.Extensions;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerDamageable : Damageable
{
	private AudioSource _source;
	private ChromaticAberration _ca;

	[SerializeField] private Volume _volume;

	protected override void Awake()
	{
		base.Awake();
		_source = GetComponent<AudioSource>();

		_volume.profile.TryGet(out _ca);
	}

	public override void ApplyDamage(GameObject attacker, float damage)
	{
		base.ApplyDamage(attacker, damage);
		GameLoop.Camera.Shake(Vector3.one * .4f, 0.2f);
		_source.Play();
	}

	public override void Kill(GameObject attacker)
	{
		base.Kill(attacker);

		float timeScale = .4f;

		Time.timeScale = timeScale;
		AudioManager.SetVolume(0.8f, 0.5f * timeScale);
		AudioManager.SetPitch(0.8f, 0.5f * timeScale);
		Tween.Value(GameLoop.Camera.BaseFov, GameLoop.Camera.BaseFov * 0.5f, (fov) => GameLoop.Camera.Zoom(fov), 2f * timeScale, 0, Tween.EaseOut);
		Tween.Value(_ca.intensity.value, 1f, (value) => _ca.intensity.value = value, 2f * timeScale, 0, Tween.EaseOut);
		InvokeRepeating(nameof(SpawnRandomExplosion), 0, 0.2f * timeScale);
		Invoke(nameof(EndGame), 2f * timeScale);
	}

	private void SpawnRandomExplosion()
	{
		GameLoop.Camera.Shake(Vector3.one * .4f, 0.3f);
		Blast.Spawn(transform.position + Random.insideUnitCircle.ToVector3XY() * 1.5f, Blast.Size.BIG);
	}

	private void EndGame()
	{
		Time.timeScale = 1f;
		Tween.Value(_ca.intensity.value, .1f, (value) => _ca.intensity.value = value, .2f, 0, Tween.EaseOut);
		CancelInvoke(nameof(SpawnRandomExplosion));
		CancelInvoke(nameof(EndGame));
		GameLoop.EndGame();
	}
}
