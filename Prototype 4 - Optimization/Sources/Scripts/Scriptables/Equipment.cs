using Sirenix.OdinInspector;
using System;
using UnityEngine;

public abstract class Equipment : IdentifiableSO
{
	[NonSerialized] protected EntityIdentity _user;
	public bool IsEquipped => _user != null;
	public abstract int UpgradeCount { get; }
	public abstract int CurrentUpgrade { get; }
	public abstract bool HasNextUpgrade { get; }

	public virtual void Reset()
	{
		_user = null;
	}

	public virtual void OnEquip(EntityIdentity user)
	{
		_user = user;
	}

	public virtual void OnUpdate() { }

	public abstract void OnUpgrade();
}
