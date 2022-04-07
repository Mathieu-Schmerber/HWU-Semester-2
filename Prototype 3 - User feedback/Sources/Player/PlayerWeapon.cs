using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWeapon : MonoBehaviour, IAnimationReceiver
{
	[SerializeField] private Transform _weaponJoint; 

	private EntityIdentity _identity;
	private Animator _animator;
    private InputHandler _inputs;
	private List<GameObject> _weaponAttackPool;
	private Cooldown _attackCooldown = new Cooldown()
	{
		readyOnStart = true,
		automaticReset = false,
		cooldownTime = 0
	};
	private float _minTimeBetweenAttacks = 0.1f;

	private Weapon _weapon;
	public event Action<WeaponItem> OnSuggestionChangedEvt;

	#region PROP: Weapon Suggestion
	private WeaponItem _suggestion;
	public WeaponItem Suggestion
	{
		get => _suggestion; set
		{
			if (value != _suggestion)
				OnSuggestionChangedEvt?.Invoke(value);
			_suggestion = value;
		}
	}
	#endregion

	#region Unity builtins

	private void OnEnable()
	{
		_inputs.OnInteractPressed += PickupSuggestion;
	}

	private void OnDisable()
	{
		_inputs.OnInteractPressed -= PickupSuggestion;
	}

	private void Awake()
	{
		_identity = GetComponent<EntityIdentity>();
		_weapon = GetComponentInChildren<Weapon>();
		_animator = GetComponentInChildren<Animator>();
		_inputs = GetComponent<InputHandler>();

		// TODO: create a propper pool
		_weaponAttackPool = new List<GameObject>();
	}

	private void Start()
	{
		_attackCooldown.Init();
		SetupWeaponConstraints();
	}

	protected virtual void Update()
	{
		if (WantsToAttack())
			TryAttack();
	}

	#endregion

	#region Equip

	private void PickupSuggestion()
	{
		if (Suggestion != null)
		{
			EquipWeapon(Suggestion);
			Suggestion = null;
		}
	}

	public void SuggestWeapon(WeaponItem data) => Suggestion = data;

	public void UnSuggestWeapon(WeaponItem data)
	{
		if (Suggestion == data)
			Suggestion = null;
	}

	/// <summary>
	/// Equips a weapon.
	/// </summary>
	public void EquipWeapon(WeaponItem item)
	{
		DropCurrentlyEquipped();

		item.transform.SetParent(_weaponJoint);
		item.enabled = false;
		_weapon = _weaponJoint.GetComponentInChildren<Weapon>(true);
		_weapon.transform.localPosition = Vector3.zero;
		_weapon.transform.localRotation = Quaternion.Euler(-90, 0, 0);
		_weapon.enabled = true;

		SetupWeaponConstraints();
	}

	private void DropCurrentlyEquipped()
	{
		_weaponJoint.GetComponentInChildren<WeaponItem>(true).enabled = true;
		_weaponJoint.GetComponentInChildren<Weapon>(true).enabled = false;
		_weaponJoint.DetachChildren();
	}

	public void SetupWeaponConstraints()
	{
		_animator.SetLayerWeight(_animator.GetLayerIndex("DefaultLocomotion"), 0f);
		_animator.SetLayerWeight(_animator.GetLayerIndex("GreatSwordLocomotion"), 0f);
		_animator.SetLayerWeight(_animator.GetLayerIndex(_weapon.Data.LocomotionLayer), 1f);
	}

	#endregion

	public PlayerAttack SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		GameObject pulled = _weaponAttackPool.FirstOrDefault(x => x.activeSelf == false && x.name == prefab.name);

		if (pulled == null)
		{
			pulled = Instantiate(prefab.gameObject, position, rotation);
			pulled.name = prefab.name;
			_weaponAttackPool.Add(pulled);
		}
		pulled.transform.position = position;
		pulled.transform.rotation = rotation;
		pulled.SetActive(true);

		PlayerAttack attack = pulled.GetComponentInChildren<PlayerAttack>();
		attack.Init(_identity);
		return attack;
	}

	#region Attack

	/// <summary>
	/// Determines if the entity is willing to try attacking <br></br>
	/// Determined by an input beiing pressed for the player for exemple.
	/// </summary>
	/// <returns></returns>
	private bool WantsToAttack() => _inputs.IsAttackDown;

	protected virtual void TryAttack()
	{
		if (_attackCooldown.IsOver())
		{
			OnMeleeAttack();
			_attackCooldown.Reset();
		}
	}

	/// <summary>
	/// Check if a combo attack is possible and adapt attack cooldown to the clip's length
	/// </summary>
	protected virtual void OnMeleeAttack()
	{
		bool canCombo = (Time.time - _attackCooldown.LastOverTime - _attackCooldown.cooldownTime) <= _weapon.ComboIntervalTime;
		var attack = _weapon.GetNextAttack(canCombo);

		if (attack == null)
			return;
		_weapon.OnAttackStart();
		_attackCooldown.SetCooldown(attack.AttackAnimation.length / _weapon.Data.AttackSpeed + _minTimeBetweenAttacks);
		_animator.Play(attack.AttackAnimation.name);
	}

	public void OnAnimationEvent(string animationArg) => _weapon.OnAnimationEvent(animationArg);

	public void OnAnimationEnter(AnimatorStateInfo stateInfo) => _weapon.OnAnimationEnter(stateInfo);

	public void OnAnimationExit(AnimatorStateInfo stateInfo) => _weapon.OnAnimationExit(stateInfo);

	#endregion
}
