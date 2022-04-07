using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightOnDamage : MonoBehaviour, IDamageableListener
{
    [SerializeField] private Material _highlight;
	[SerializeField] private float _highlightTime;

	private Material[] _defaultMaterials;
	private Renderer _renderer;

	private void Awake()
	{
		_renderer = GetComponentInChildren<Renderer>();
		_defaultMaterials = _renderer.materials;
	}

	private IEnumerator Highlight()
	{
		Material[] currentMaterials = _renderer.materials;
		for (int i = 0; i < currentMaterials.Length; i++)
			currentMaterials[i] = _highlight;
		_renderer.materials = currentMaterials;
		yield return new WaitForSeconds(_highlightTime);
		_renderer.materials = _defaultMaterials;
	}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) => StartCoroutine(Highlight());

	public void OnDeath(GameObject attacker, IDamageProcessor victim){}
}
