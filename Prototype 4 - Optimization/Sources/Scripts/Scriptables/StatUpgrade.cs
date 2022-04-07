using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/StatUpgrade")]
public class StatUpgrade : UpgradableEquipment<StatUpgrade.StatUnit>
{
	[System.Serializable]
	public class StatUnit
	{
		public int MaxHealthUnit;
		public float RegenerationUnit;
		public float SpeedUnit;
		public float ManiabilityUnit;
		public float DamageMultiplierUnit;
	}

	public override void OnEquip(EntityIdentity user)
	{
		base.OnEquip(user);
		ApplyStatGain();
	}

	public override void OnUpgrade()
	{
		base.OnUpgrade();
		ApplyStatGain();
	}

	private void ApplyStatGain()
	{
		_user.MaxHealth.BonusValue = _currentUpgrade.MaxHealthUnit;
		_user.Regeneration.BonusValue = _currentUpgrade.RegenerationUnit;
		_user.Speed.BonusValue = _currentUpgrade.SpeedUnit;
		_user.Maniability.BonusValue = _currentUpgrade.ManiabilityUnit;
		_user.DamageMultiplier.BonusValue = _currentUpgrade.DamageMultiplierUnit;
	}
}