using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SheepAI : SignalEmitter
{
	#region Properties

	[SerializeField] private LayerMask _wallLayer;
	[SerializeField] private AudioClip[] _audioClips;

	private NavMeshAgent _agent;
	private Animator _animator;
	private SheepAI _leader;
	private AudioSource _source;

	private bool _hasDestination => !_agent.isStopped;
	private bool _isLeader => _leader == null;
	private bool _isFollowing => _leader != null;
	public float Speed => _agent.speed;

	#endregion

	#region Unity Built-in

	private void Awake()
	{
		_source = GetComponent<AudioSource>();
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		_agent.isStopped = HasReachedDestination();

		if (!_hasDestination)
			_agent.velocity = Vector3.zero;
		if (_isFollowing)
		{
			_agent.SetDestination(_leader.transform.position);
			if (!_leader._hasDestination)
			{
				_leader = null;
				_agent.isStopped = true;
				_agent.SetDestination(transform.position);
			}
		}
		else if (_isLeader && _hasDestination)
			EmitSignal(SignalType.GROUP_UP, 4);

		_animator.SetFloat("Speed", _hasDestination ? 1 : 0);
	}

	#endregion

	private bool HasReachedDestination() => Vector3.Distance(transform.position, _agent.destination) <= _agent.stoppingDistance;

	private void RunAwayFromPosition(Vector3 position)
	{
		Vector3 dir = (position - transform.position).normalized;
		Vector3 destination = transform.position - (dir * Speed);
			
		destination.y = transform.position.y;
		if (Physics.Linecast(transform.position, destination, _wallLayer))
			destination = Spawner.Instance.GetRandomPosition(transform.position.y);
		
		if (_agent != null && _agent.isActiveAndEnabled)
			_agent.SetDestination(destination);
	}

	protected override void OnSignalReceived(SignalEmitter emitter, SignalType signal)
	{
		if (Spawner.Instance != null && !Spawner.Instance.GameRunning)
			return;
		switch (signal)
		{
			case SignalType.DANGER:
				if (_isLeader)
				{
					PlayAudio();
					RunAwayFromPosition(emitter.transform.position);
				}
				break;
			case SignalType.GROUP_UP:
				if (!_hasDestination)
					_leader = (SheepAI)emitter;
				break;
			default:
				break;
		}
	}

	public void PlayAudio()
	{
		if (!_source.isPlaying)
			_source.PlayOneShot(_audioClips.Random(), 1);
	}

	private void OnDestroy() => EmitSignal(SignalType.DANGER, 7f);

	#region Debug

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			if (_hasDestination)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, _agent.destination);
			}
		}
	}

	#endregion
}
