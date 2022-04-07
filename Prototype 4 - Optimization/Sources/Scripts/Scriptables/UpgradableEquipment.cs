using Sirenix.OdinInspector;
using System;
using UnityEngine;

public abstract class UpgradableEquipment<T> : Equipment
{
	[SerializeField] protected T[] _upgrades;
	protected T _currentUpgrade => _upgrades[_currentUpgradeIndex];
	[NonSerialized] protected int _currentUpgradeIndex;

	public override int UpgradeCount => _upgrades.Length;
	public override int CurrentUpgrade => _currentUpgradeIndex;
	public override bool HasNextUpgrade => _currentUpgradeIndex + 1 < UpgradeCount;

	public override void OnEquip(EntityIdentity user)
	{
		base.OnEquip(user);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override void OnUpgrade()
	{
		if (HasNextUpgrade)
			_currentUpgradeIndex++;
	}

	public override void Reset()
	{
		base.Reset();
		_currentUpgradeIndex = 0;
	}
}
