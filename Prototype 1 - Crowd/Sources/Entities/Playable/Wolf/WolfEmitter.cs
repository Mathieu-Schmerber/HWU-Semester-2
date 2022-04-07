using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfEmitter : SignalEmitter
{
	protected Animator _animator;

	protected virtual void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	protected override void OnSignalReceived(SignalEmitter emitter, SignalType signal) { }

	protected virtual void Update() => EmitSignal(SignalType.DANGER, 7);

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (!this.enabled) return;

		SheepAI sheep = collision.gameObject.GetComponent<SheepAI>();

		if (sheep)
			Spawner.Instance.KillSheep(sheep);
	}
}
