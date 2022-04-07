using Sirenix.OdinInspector;
using System;
using UnityEngine;

public abstract class IdentifiableSO : ScriptableObject
{
	[ReadOnly] public string ID = string.Empty;

	/// <summary>
	/// On create/compile
	/// </summary>
	protected virtual void Awake()
	{
		if (string.IsNullOrEmpty(ID))
			GenerateID();
	}

	[Button, ShowIf("@string.IsNullOrEmpty(ID)")]
	private void GenerateID() => ID = Guid.NewGuid().ToString();
}