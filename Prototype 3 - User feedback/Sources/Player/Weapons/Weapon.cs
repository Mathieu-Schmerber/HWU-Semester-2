using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	#region Properties

	[SerializeField] private WeaponData _data;
	public WeaponData Data { get => _data; set => _data = value; }

	protected AController _controller;
	protected PlayerWeapon _weaponManager;
	protected int _lastAttackIndex = -1;

	/// <summary>
	/// Returns the current attack
	/// </summary>
	public WeaponData.WeaponAttack CurrentWeaponAttack
	{
		get => _data.AttackCombos[_lastAttackIndex];
	}

	/// <summary>
	/// Returns the time to input between attack animations to perform a combo
	/// </summary>
	public float ComboIntervalTime { get => _data.ComboIntervalTime; }

	#endregion

	#region Unity builtins

	private void OnEnable()
	{
		_controller = GetComponentInParent<AController>();
		_weaponManager = GetComponentInParent<PlayerWeapon>();
		_lastAttackIndex = 0;
	}

	#endregion

	private void FaceClosestTarget(float range, float maxAngle)
	{
		Transform[] inRange = Physics.OverlapSphere(_controller.transform.position, range)
			.Where(x => x.gameObject != _controller.gameObject && x.GetComponent<IDamageProcessor>() != null)
			.Select(x => x.transform)
			.OrderBy(x => Vector3.Distance(_controller.transform.position, x.position)).ToArray();

		if (inRange.Length == 0) return;
		foreach (Transform entity in inRange)
		{
			Vector3 dir = (entity.position - _controller.transform.position).normalized;
			float angle = Vector3.Angle(_controller.GetAimNormal(), dir);

			if (angle <= maxAngle)
			{
				_controller.LockTarget(entity, true);
				return;
			}
		}
	}

	protected void ActivateAimAssist(float range, float angleAssist) => FaceClosestTarget(range, angleAssist);
	protected void DeactivateAimAssist() =>_controller.UnlockTarget();

	public virtual void OnAttackStart() { }

	/// <summary>
	/// Returns the next attack
	/// </summary>
	/// <param name="continueCombo">Should it get the next combo part ?</param>
	/// <returns></returns>
	public virtual WeaponData.WeaponAttack GetNextAttack(bool continueCombo = true)
	{
		if (_lastAttackIndex + 1 < _data.AttackCombos.Count && continueCombo)
		{
			_lastAttackIndex++;
			return CurrentWeaponAttack;
		}
		_lastAttackIndex = 0;
		return _data.AttackCombos.First();
	}

	public virtual void OnAnimationEvent(string animation)
	{
		if (animation == "Attack")
		{
			PlayerAttack pa = _weaponManager.SpawnFromPool(CurrentWeaponAttack.Attack.Prefab, _controller.transform.position, Quaternion.identity);

			if (CurrentWeaponAttack.Attack.AimAssist)
				ActivateAimAssist(pa.Range, 50);

			Vector3 aimDir = _controller.GetAimNormal();
			bool canDash = (CurrentWeaponAttack.Dash.Distance > 0 &&
						   (_controller.GetMovementNormal().magnitude > 0 || !CurrentWeaponAttack.Dash.OnlyWhenMoving) &&
						   !CurrentWeaponAttack.Dash.OnAnimationEventOnly);
			Vector3 dashOffset = canDash && !pa.FollowCaster ? pa.transform.forward * CurrentWeaponAttack.Dash.Distance : Vector3.zero;

			pa.transform.rotation = Quaternion.LookRotation(aimDir);
			pa.OnStart(CurrentWeaponAttack.Attack.StartOffset + dashOffset, CurrentWeaponAttack.Attack.TravelDistance);
			if (CurrentWeaponAttack.Dash.Distance > 0 && 
				(_controller.GetMovementNormal().magnitude > 0 || !CurrentWeaponAttack.Dash.OnlyWhenMoving) && 
				!CurrentWeaponAttack.Dash.OnAnimationEventOnly)
				_controller.Dash(aimDir, CurrentWeaponAttack.Dash.Distance, CurrentWeaponAttack.Dash.Duration, CurrentWeaponAttack.Dash.AfterImages);

			if (CurrentWeaponAttack.Attack.AimAssist)
				DeactivateAimAssist();
		}
		else if (animation == "Dash" && CurrentWeaponAttack.Dash.OnAnimationEventOnly &&
				(_controller.GetMovementNormal().magnitude > 0 || !CurrentWeaponAttack.Dash.OnlyWhenMoving))
			_controller.Dash(_controller.GetAimNormal(), CurrentWeaponAttack.Dash.Distance, CurrentWeaponAttack.Dash.Duration, CurrentWeaponAttack.Dash.AfterImages);
	}

	public virtual void OnAnimationEnter(AnimatorStateInfo stateInfo) { _controller.CanMove = false; }

	public virtual void OnAnimationExit(AnimatorStateInfo stateInfo) {
		_controller.CanMove = true;
		_controller.IsAimLocked = false;
	}
}
