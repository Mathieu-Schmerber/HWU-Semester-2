using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfStealth : MonoBehaviour
{
    private MeshRenderer[] _renderers;
	private Transform _gfx;
	private bool _inStealth;
	private bool _visible;

	private void Awake()
	{
		_renderers = GetComponentsInChildren<MeshRenderer>();
		_gfx = GetComponentInChildren<Animator>().transform;
	}

	private void OnEnable()
	{
		if (!this.enabled) return;

		DetectionLight.OnSignalEmitterEnter += DetectionLight_OnSignalEmitterEnter;
		DetectionLight.OnSignalEmitterLeave += DetectionLight_OnSignalEmitterLeave;
	}

	private void OnDisable()
	{
		if (!this.enabled) return;

		DetectionLight.OnSignalEmitterEnter -= DetectionLight_OnSignalEmitterEnter;
		DetectionLight.OnSignalEmitterLeave -= DetectionLight_OnSignalEmitterLeave;
	}

	private void DetectionLight_OnSignalEmitterEnter(SignalEmitter signalEmitter)
	{
		if (!this.enabled) return;

		if (signalEmitter.gameObject == gameObject)
		{
			_visible = true;
			SetStealth(false);
		}
	}

	private void DetectionLight_OnSignalEmitterLeave(SignalEmitter signalEmitter)
	{
		if (!this.enabled) return;

		if (signalEmitter.gameObject == gameObject)
		{
			_visible = false;
			SetStealth(true);
		}
	}

	public void SetStealth(bool state)
	{
		if (!this.enabled) return;

		_inStealth = state;
		foreach (var item in _renderers)
			item.enabled = !_inStealth;
	}

	public void QuitStealthMomentarily(float time) => StartCoroutine(nameof(TimedUnstealth), time);

	private IEnumerator TimedUnstealth(float time)
	{
		if (this.enabled == false) yield return null;

		if (!_visible)
			SetStealth(false);
		yield return new WaitForSeconds(time);
		if (!_visible)
			SetStealth(true);
	}
}
