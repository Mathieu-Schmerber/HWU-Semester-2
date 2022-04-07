using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SignalEmitter : MonoBehaviour
{
	public enum SignalType
	{
		DANGER = 0,
		GROUP_UP
	}

	/// <summary>
	/// Emits a signal around the entity
	/// </summary>
	/// <param name="signal">The type of signal to be emitting</param>
	/// <param name="radius">The range of the signal</param>
    protected void EmitSignal(SignalType signal, float radius)
	{
		List<GameObject> colliders = Physics.OverlapSphere(transform.position, radius).Select(x => x.gameObject).ToList();

		colliders.Remove(gameObject);
		foreach (GameObject collider in colliders)
			collider.GetComponent<SignalEmitter>()?.OnSignalReceived(this, signal);
	}

	/// <summary>
	/// Triggers whenever the entity receives a signal
	/// </summary>
	/// <param name="emitter">The emitter of the received signal</param>
	/// <param name="signal">The type of the received signal</param>
	protected abstract void OnSignalReceived(SignalEmitter emitter, SignalType signal);
}
