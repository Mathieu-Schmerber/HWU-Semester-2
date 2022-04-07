using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Tesla")]
public class Tesla : UpgradableEquipment<Tesla.Upgrade>
{
	[System.Serializable]
	public struct Upgrade
	{
		public float Damage;
		public float Range;
		public float AttackRate;
	}

	[SerializeField] private GameObject _teslaPrefab;
	public float Range => _currentUpgrade.Range;
	public float AttackRate => _currentUpgrade.AttackRate;
	public float Damage => _currentUpgrade.Damage;

	public override void OnEquip(EntityIdentity user)
	{
		base.OnEquip(user);
		GameObject go = Instantiate(_teslaPrefab, user.transform);
		go.GetComponent<TeslaWeapon>().Init(this);
	}
}