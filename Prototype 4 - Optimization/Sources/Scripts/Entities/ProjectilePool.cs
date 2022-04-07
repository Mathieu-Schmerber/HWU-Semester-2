using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nawlian.Lib.Systems.Pooling;

[RequireComponent(typeof(Projectile), typeof(Mine), typeof(Missile))]
public class ProjectilePool : APoolableObject
{
	public struct InitData
	{
		public EntityIdentity shooter;
		public ProjectileData data;
	}

	private Projectile _projectile;
	private Mine _mine;
	private Missile _missile;

	private void Awake()
	{
		_projectile = GetComponent<Projectile>();
		_mine = GetComponent<Mine>();
		_missile = GetComponent<Missile>();
		_projectile.enabled = false;
		_mine.enabled = false;
		_missile.enabled = false;
	}

	public override void Init(object data)
	{
		InitData proj = (InitData)data;

		switch (proj.data.Type)
		{
			case ProjectileData.ProjectileType.PROJECTILE:
				_projectile.Init(proj);
				_projectile.enabled = true;
				break;
			case ProjectileData.ProjectileType.MINE:
				_mine.Init(proj);
				_mine.enabled = true;
				break;
			case ProjectileData.ProjectileType.MISSILE:
				_missile.Init(proj);
				_missile.enabled = true;
				break;
		}
		Invoke(nameof(Release), proj.data.BaseLifetime);
	}

	protected override void OnReleasing()
	{
		base.OnReleasing();

		CancelInvoke(nameof(Release));
		if (_projectile.enabled)
			_projectile.OnLifetimeEnd();
		if (_mine.enabled)
			_mine.OnLifetimeEnd();
		if (_missile.enabled)
			_missile.OnLifetimeEnd();
		_projectile.enabled = false;
		_mine.enabled = false;
		_missile.enabled = false;
	}

	public static void Spawn(Vector3 position, Quaternion rotation, EntityIdentity shooter, ProjectileData data) 
		=> ObjectPooler.Get(PoolIdEnum.PROJECTILE, position, rotation, new InitData() { shooter = shooter, data = data});
}
