using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelsoftGames.PixelUI;

public class EntityTooltipOnSelect : MonoBehaviour, ISelectableListener, IDamageableListener
{
	private EntityType _identity;

	private void Awake()
	{
		_identity = GetComponent<EntityType>();
	}

	public void OnSelect()
	{
		TooltipManager.Instance.Show(TooltipManager.TooltipType.STATS, gameObject, _identity);
	}

	public void OnDeselect()
	{
		TooltipManager.Instance.Hide(TooltipManager.TooltipType.STATS);
	}

	public void OnKeepSelecting() {}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) {}

	public void OnDeath(GameObject attacker, IDamageProcessor victim) => OnDeselect();
}
