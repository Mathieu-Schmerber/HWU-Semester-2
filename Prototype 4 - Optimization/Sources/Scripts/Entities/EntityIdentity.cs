using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class EntityIdentity : MonoBehaviour
{
	#region Types

	[System.Serializable]
	public class StatLine
	{
		/// <summary>
		/// Natural stat, immutable
		/// </summary>
		public float BaseValue { get; set; }

		/// <summary>
		/// Additional stat active for an undefined time.
		/// </summary>
		/// <remarks>For example: Equipments and items can provide stats, which will be removed whenever unequiped.</remarks>
		[HideInInspector] public float BonusValue;

		/// <summary>
		/// Additional stat active for a defined time
		/// </summary>
		/// <remarks>For example: A passive or a spell effect will provide stats for a defined time.</remarks>
		[HideInInspector] public float TemporaryValue;

		/// <summary>
		/// Total value, temporary buffs included
		/// </summary>
		[ShowInInspector, ReadOnly] public float Value { get => BaseValue + BonusValue + TemporaryValue; }

		/// <summary>
		/// Static value, temporary buffs excluded
		/// </summary>
		public float StaticValue { get => BaseValue + BonusValue; }
	}

	#endregion

	public EntityStatData Stats;

	private float _currentHealth;

	public int CoinWorth { get; set; }
	public bool IsPlayer => Stats.IsPlayerTeam;
	public float CurrentHealth { get => _currentHealth; 
		set {
			float clamp = Mathf.Clamp(value, 0, (int)MaxHealth.Value);
			_currentHealth = clamp;
			OnCurrentHealthValueChanged?.Invoke(_currentHealth);
		}
	}

	[ReadOnly] public StatLine MaxHealth;
	[ReadOnly] public StatLine Regeneration;
	[ReadOnly] public StatLine Speed;
	[ReadOnly] public StatLine Maniability;
	[ReadOnly] public StatLine DamageMultiplier;

	private float _lastRegenFrame;
	private float _regenRate => Regeneration.Value == 0 ? 0 : 1 / Regeneration.Value;

	public event Action<float> OnCurrentHealthValueChanged;
	
	private void OnEnable()
	{
		GameLoop.OnGameStarted += InitStats;
		InitStats();
	}

	private void OnDisable()
	{
		GameLoop.OnGameStarted -= InitStats;
	}

	public void InitStats() => InitStats(1f);

	public void InitStats(float rawScaling)
	{
		if (Stats == null)
			return;
		MaxHealth.BaseValue = Stats.BaseMaxHealth * rawScaling;
		Regeneration.BaseValue = Stats.BaseRegeneration * rawScaling;
		DamageMultiplier.BaseValue = Stats.BaseDamageMultiplier * rawScaling;
		Maniability.BaseValue = Stats.BaseManiability * rawScaling;
		Speed.BaseValue = Mathf.Clamp(Stats.BaseSpeed * rawScaling * 0.7f, 0f, 10f); // Speed scaling is 30% slower (0.7f) and maxed at 10f
		CurrentHealth = (int)MaxHealth.Value;
		CoinWorth = (int)(Stats.CoinWorth * rawScaling);

		_lastRegenFrame = 0;
	}

	private void Update()
	{
		if (Regeneration.Value == 0) return;
		else if (Time.time - _lastRegenFrame >= _regenRate)
		{
			_lastRegenFrame = Time.time;
			CurrentHealth += 1;
		}
	}
}