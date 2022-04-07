using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfAI : WolfEmitter
{
	#region Properties

	private NavMeshAgent _agent;
	private Transform _target;
	private WolfStealth _stealth;

	public float Speed => _agent.speed;

	#endregion

	#region Unity Built-in

	protected sealed override void Awake()
	{
		base.Awake();
		_stealth = GetComponent<WolfStealth>();
		_agent = GetComponent<NavMeshAgent>();
	}

	protected override void Update()
	{
		base.Update();

		CalculateDestination();
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		SheepAI sheep = collision.gameObject.GetComponent<SheepAI>();

		if (sheep)
		{
			Spawner.Instance.KillSheep(sheep);
			_stealth.QuitStealthMomentarily(0.5f);
		}
		if (_target == collision.transform)
			_target = null;
	}

	#endregion

	private void CalculateDestination()
	{
		if (_target == null)
			_target = Spawner.Instance.GetClosestSheep(transform.position);
		_animator.SetFloat("Speed", _agent.remainingDistance <= _agent.stoppingDistance ? 0 : 1);
		_agent.SetDestination(_target.position);
	}

	protected override void OnSignalReceived(SignalEmitter emitter, SignalType signal) {}
}
