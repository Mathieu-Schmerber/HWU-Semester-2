using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlineOnSelect : MonoBehaviour, ISelectableListener, IDamageableListener
{
	private Outline _outline;
	private float _thickness;

	private void Awake()
	{
		_outline = GetComponent<Outline>();
		_outline.enabled = false;
	}

	public void OnSelect()
	{
		_outline.enabled = true;
	}

	public void OnDeselect()
	{
		_outline.enabled = false;
	}

	public void OnKeepSelecting() { }

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) { }

	public void OnDeath(GameObject attacker, IDamageProcessor victim) => OnDeselect();

}
