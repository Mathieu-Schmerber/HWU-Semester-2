using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages every tooltips
/// </summary>
public class TooltipManager : Singleton<TooltipManager>
{
	#region Types

	public enum TooltipType
	{
		STATS
	}

	[System.Serializable] public class TooltipObjectType : UnitySerializedDictionary<TooltipType, Tooltip> { }

	#endregion

	[SerializeField] private TooltipObjectType _tooltips;
	[SerializeField] private Vector3 _padding;

	public void Show(TooltipType type, GameObject sender, object value)
	{
		_tooltips[type]?.gameObject.SetActive(true);
		_tooltips[type]?.SetPosition(sender, _padding);
		_tooltips[type]?.Bind(value);
	}

	public void Hide(TooltipType type)
	{
		_tooltips[type]?.gameObject.SetActive(false);
	}
}
