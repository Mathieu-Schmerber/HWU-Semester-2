using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Gun")]
public class Gun : UpgradableEquipment<Gun.ShootStage>
{
	#region Types

	[System.Serializable]
	public class ShootStage
	{
		public ShootingLine[] ShootingLines;
		public float ShootCooldown;
		[InlineEditor] public ProjectileData Projectile;
	}

	[System.Serializable]
	public class ShootingLine
	{
		public float ShootAngle;
		public Vector3 ShootOffset;
	}

	#endregion

	private Timer _shootTimer = new Timer();

	public override void OnEquip(EntityIdentity user)
	{
		base.OnEquip(user);
		_shootTimer.Start(_currentUpgrade.ShootCooldown, true, Shoot);
	}

	private void Shoot()
	{
		if (!GameLoop.HasGameStarted) return;
		foreach (ShootingLine line in _currentUpgrade.ShootingLines)
		{
			Vector3 offset = _user.transform.InverseTransformDirection(line.ShootOffset);

			ProjectilePool.Spawn(
				position: _user.transform.position + offset,
				rotation: Quaternion.Euler(0, 0, _user.transform.rotation.eulerAngles.z + line.ShootAngle),
				shooter: _user,
				data: _currentUpgrade.Projectile
			);
		}
	}
}