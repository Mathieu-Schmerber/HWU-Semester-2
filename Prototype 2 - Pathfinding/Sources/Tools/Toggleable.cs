using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Defines an In-Editor toggleable
/// </summary>
[System.Serializable, Toggle(nameof(Toggleable.Enabled), CollapseOthersOnExpand = false)]
public class Toggleable
{
	public bool Enabled = true;
}

/// <summary>
/// Defines an In-Editor toggleable hosting a single value
/// </summary>
[System.Serializable]
public class ToggleableValue<T> : Toggleable
{
	[SerializeField] private T _value;

	public T Value
	{
		get { return _value; }
		set { _value = value; }
	}

	public static implicit operator ToggleableValue<T>(T b) => new ToggleableValue<T>() { Value = b};
}