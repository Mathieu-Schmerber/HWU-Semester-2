using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AController, IAnimationReceiver, IDamageableListener
{
	[SerializeField] protected float _detectionRange;

	private Damageable _damageable;
	private EnemyAttack _attack;
	private PlayerController _player;
	private bool _isAttacking;

	protected override void Awake()
	{
		base.Awake();

		// TODO: temporary, get it from some manager instead
		_player = GameObject.FindObjectOfType<PlayerController>();
		_attack = GetComponentInChildren<EnemyAttack>(includeInactive: true);
		_damageable = GetComponent<Damageable>();
	}

	protected override void Update()
	{
		base.Update();
		if (_lockedTarget == null && Vector3.Distance(transform.position, _player.transform.position) <= _detectionRange)
			LockTarget(_player.transform);
		else if (_lockedTarget != null && !_isAttacking && Vector3.Distance(transform.position, _lockedTarget.position) <= _attack.Range)
			_gfxAnim.Play("Punch");
	}

	protected override Vector3 GetMovementsInputs()
	{
		if (_lockedTarget == null || _isAttacking)
			return Vector3.zero;
		// Move straight to the target
		return GetAimNormal();
	}

	protected override Vector3 GetTargetPosition() => transform.position;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, _detectionRange);
	}

	public void OnAnimationEvent(string animationArg)
	{
		if (animationArg == "Attack")
		{
			CanMove = false;
			IsAimLocked = true;
			_attack.gameObject.SetActive(true);
		}
	}

	public void OnAnimationEnter(AnimatorStateInfo stateInfo)
	{
		_isAttacking = stateInfo.IsName("Punch");
	}

	public void OnAnimationExit(AnimatorStateInfo stateInfo)
	{
		_isAttacking = !stateInfo.IsName("Punch");
		CanMove = true;
		IsAimLocked = false;
	}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount)
	{
	}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		this.enabled = false;
	}
}
