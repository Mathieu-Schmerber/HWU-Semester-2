using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Defines the ability to select an entity with the mouse.
/// </summary>
public class EntityMouseSelector : MonoBehaviour, IDamageableListener
{
	private List<ISelectableListener> _selectable;

	private void Awake()
	{
		_selectable = GetComponents<ISelectableListener>().ToList();
	}

	private void OnMouseOver()
	{
		if (enabled)
			_selectable.ForEach(x => x.OnKeepSelecting());
	}

	private void OnMouseExit()
	{
		if (enabled)
			_selectable.ForEach(x => x.OnDeselect());
	}

	private void OnMouseEnter()
	{
		if (enabled)
			_selectable.ForEach(x => x.OnSelect());
	}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) {}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		_selectable.ForEach(x => x.OnDeselect());
		enabled = false;
	}
}
