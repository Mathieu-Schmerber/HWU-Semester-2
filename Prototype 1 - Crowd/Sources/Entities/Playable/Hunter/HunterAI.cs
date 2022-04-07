using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterAI : HunterEmitter
{
    private NavMeshAgent _agent;
	private WolfEmitter _target;

	protected override void Awake()
	{
		base.Awake();
		_agent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		DetectionLight.OnSignalEmitterEnter += DetectionLight_OnSignalEmitterEnter;
		DetectionLight.OnSignalEmitterLeave += DetectionLight_OnSignalEmitterLeave;
	}

	private void OnDestroy()
	{
		DetectionLight.OnSignalEmitterEnter -= DetectionLight_OnSignalEmitterEnter;
		DetectionLight.OnSignalEmitterLeave -= DetectionLight_OnSignalEmitterLeave;
	}

	private void DetectionLight_OnSignalEmitterLeave(SignalEmitter signalEmitter)
	{
		if (signalEmitter is WolfEmitter)
			_target = null;
	}

	private void DetectionLight_OnSignalEmitterEnter(SignalEmitter signalEmitter)
	{
		if (signalEmitter is WolfEmitter)
			_target = signalEmitter as WolfEmitter;
	}

	protected override void Update()
    {
		base.Update();

		_agent.isStopped = HasReachedDestination();
		if (HasReachedDestination() && _shoot.CanMove)
			_agent.SetDestination(Spawner.Instance.GetRandomPositionAroundLight(transform.position.y));

		if (_target != null)
		{
			_shoot.AimAt(_target.transform.position);
			_agent.SetDestination(transform.position);
		}
		_animator.SetFloat("Speed", 0.5f);
	}

	private bool HasReachedDestination() => Vector3.Distance(transform.position, _agent.destination) <= _agent.stoppingDistance;
}
