using UnityEngine;

[CreateAssetMenu(menuName = "Data/Projectile")]
public class ProjectileData : ScriptableObject
{
	public enum ProjectileType
	{
		PROJECTILE,
		MINE,
		MISSILE
	}

	public ProjectileType Type;
	public Sprite Graphics;
	public float BaseDamage;
	public float BaseSpeed;
	public float BaseLifetime;
	public Vector3 Scale;
}