using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EntityIdentity : EntityType, IDamageableListener
{
	#region Types

	[System.Serializable]
	public class StatLine : Toggleable
	{
		#region Value
		private int _value;

		public int Value
		{
			get => _value;
			set => _value = Mathf.Min(value, Max);
		}
		#endregion
		public int Max;

		public StatLine() { }
		public StatLine(StatLine other)
		{
			Max = other.Max;
			Value = other.Max;
			Enabled = other.Enabled;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	#endregion

	#region Fields

	[SerializeField] private EntityData _identityData;

	private StatLine _health;
	private StatLine _movementPoints;
	private StatLine _range;
	private StatLine _damage;

	private StatLine _woodGain;
	private StatLine _metalGain;

	private GameObject _resourcePrefab;

	#endregion

	public EntityData Identity => _identityData;

	public Guid Guid { get; set; }

	[DisplayName("$NAME")]
	public string Name => _identityData.Name;

	public Sprite Icon => _identityData.Icon;
	public bool DropsResources => _identityData.DropsResource.Enabled;
	public bool IsBuild => _identityData.IsBuild.Enabled;

	[DisplayName("$HP")]
	public StatLine Health
	{
		get { return _health; }
		set { _health = value; }
	}

	[DisplayName("$MP")]
	public StatLine MovementPoints
	{
		get { return _movementPoints; }
		set { _movementPoints = value; }
	}

	[DisplayName("$RNG")]
	public StatLine Range
	{
		get { return _range; }
		set { _range = value; }
	}

	[DisplayName("$DMG")]
	public StatLine Damage
	{
		get { return _damage; }
		set { _damage = value; }
	}

	[DisplayName("$WOOD")]
	public StatLine WoodGain
	{
		get { return _woodGain; }
		set { _woodGain = value; }
	}

	[DisplayName("$METAL")]
	public StatLine MetalGain
	{
		get { return _metalGain; }
		set { _metalGain = value; }
	}

	protected virtual void Awake()
	{
		Guid = Guid.NewGuid();

		Health = _identityData.Health.Enabled ? new StatLine(_identityData.Health) : null;
		MovementPoints = _identityData.MovementPoints.Enabled ? new StatLine(_identityData.MovementPoints) : null;
		Range = _identityData.Range.Enabled ? new StatLine(_identityData.Range) : null;
		Damage = _identityData.Damage.Enabled ? new StatLine(_identityData.Damage) : null;

		_resourcePrefab = ResourceLoader.GetResourcePrefab();
	}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount) {}

	public virtual void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		if (_identityData.DropsResource.Enabled)
		{
			GameObject instance = EntityMap.Instance.SpawnEntity(_resourcePrefab, CurrentNode);
			ResourceEntity entity = instance.GetComponent<ResourceEntity>();

			entity.Init(_identityData.DropsResource.Value);
		}
		EntityMap.Instance.RemoveEntity(this);
	}
}