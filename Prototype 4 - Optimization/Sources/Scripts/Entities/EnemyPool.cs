using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : APoolableObject
{
	public enum EnemyType
	{
		SEEKER, SHOOTER
	}

	private AEnemyController[] _behaviours;
	private EntityIdentity _identity;
	private SpriteRenderer _renderer;

	private void Awake()
	{
		_behaviours = GetComponents<AEnemyController>();
		_identity = GetComponent<EntityIdentity>();
		_renderer = GetComponent<SpriteRenderer>();
	}

	public override void Init(object data)
	{
		EnemyData enemy = (EnemyData)data;

		Type comp = null;

		switch (enemy.Behaviour)
		{
			case EnemyType.SEEKER:
				comp = typeof(SeekerBehaviour);
				break;
			case EnemyType.SHOOTER:
				comp = typeof(ShooterBehaviour);
				break;
		}
		_behaviours.ForEach(x => x.enabled = x.GetType() == comp);
		_identity.Stats = enemy;
		_renderer.sprite = enemy.Graphics;
	}

	public static void Spawn(Vector3 position, EnemyData data, float scalingFactor)
	{
		ObjectPooler.Get(PoolIdEnum.ENEMY, position, Quaternion.identity, data, (entity) => {
			entity.GetComponent<EntityIdentity>().InitStats(scalingFactor);
		});
	}
}