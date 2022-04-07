using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterEmitter : SignalEmitter
{
	protected HunterShoot _shoot;
	protected Animator _animator;

	protected virtual void Awake()
	{
		_shoot = GetComponent<HunterShoot>();
		_animator = GetComponentInChildren<Animator>();
	}

	protected virtual void Update()
	{
		// The sheeps will constantly make way
		EmitSignal(SignalType.DANGER, 3f);
	}

	protected override void OnSignalReceived(SignalEmitter emitter, SignalType signal) {}
}
