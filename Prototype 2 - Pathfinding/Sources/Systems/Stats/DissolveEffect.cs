using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Applies a dissolve shader effect on spawn and on death
/// </summary>
public class DissolveEffect : MonoBehaviour, IDamageableListener
{
	[System.Serializable]
	internal class DeathArgs : Toggleable
	{
		public float DelayedStart = 1f;
	}

	[SerializeField] private Shader _colorDissolve;
	[SerializeField] private Shader _textureDissolve;
	[SerializeField] private DeathArgs _onDeath;
	[SerializeField] private Toggleable _onSpawn;
	public float _deathDuration = 1f;
	public float SpawnDuration = 1f;

	private List<Renderer> _renderers = new List<Renderer>();

	private void Awake()
	{
		foreach (Renderer item in GetComponentsInChildren<MeshRenderer>())
			_renderers.Add(item);
		foreach (Renderer item in GetComponentsInChildren<SkinnedMeshRenderer>())
			_renderers.Add(item);
		_renderers.ForEach(x => ConvertToDissolvable(x));
	}

	private void Start()
	{
		if (_onSpawn.Enabled)
			FadeIn();
	}

	private IEnumerator LerpValue(float startValue, float targetValue, float duration)
	{
		float time = 0;

		while (time < duration)
		{
			_renderers.ForEach(x =>
			{
				foreach (Material item in x.materials)
					item.SetFloat("_Dissolve", Mathf.Lerp(startValue, targetValue, time / duration));
			});
			time += Time.deltaTime;
			yield return null;
		}
	}

	private void ConvertToDissolvable(Renderer renderer)
	{
		Material[] materials = renderer.materials;

		for (int i = 0; i < materials.Length; i++)
		{
			renderer.materials[i].shader = materials[i].mainTexture == null ? _colorDissolve : _textureDissolve;
			renderer.materials[i].SetTexture("_Texture2D", materials[i].mainTexture);
			renderer.materials[i].SetColor("_Color", materials[i].color);
		}
	}

	public void FadeOut()
	{
		StartCoroutine(LerpValue(0, 1, _deathDuration));
	}

	public void FadeIn() => StartCoroutine(LerpValue(1, 0, SpawnDuration));

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) {}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		if (_onDeath.Enabled)
			Invoke(nameof(FadeOut), _onDeath.DelayedStart);
	}
}
