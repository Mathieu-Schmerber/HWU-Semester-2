using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(LineRenderer))]
public class DetectionLight : MonoBehaviour
{
	[SerializeField] private int _segments;
	[SerializeField] private float _thickness;

	public delegate void EmitterEvt(SignalEmitter signalEmitter);
    public static event EmitterEvt OnSignalEmitterEnter;
    public static event EmitterEvt OnSignalEmitterLeave;
	public float Radius => _collider.radius;
	private Vector3 CenterOffset => _collider.center;

	private SphereCollider _collider;
	private LineRenderer _line;

	private void Awake()
	{
		_collider = GetComponent<SphereCollider>();
		_line = GetComponent<LineRenderer>();

		DrawCircle();
	}

	private void DrawCircle()
	{
		_line.useWorldSpace = false;
		_line.startWidth = _thickness;
		_line.endWidth = _thickness;
		_line.positionCount = _segments + 1;

		var pointCount = _segments + 1;
		var points = new Vector3[pointCount];

		for (int i = 0; i < pointCount; i++)
		{
			var rad = Mathf.Deg2Rad * (i * 360f / _segments);
			points[i] = new Vector3(Mathf.Sin(rad) * Radius, 0, Mathf.Cos(rad) * Radius) + CenterOffset;
		}

		_line.SetPositions(points);
	}

	private void OnTriggerEnter(Collider other)
	{
		SignalEmitter emitter = other.gameObject.GetComponent<SignalEmitter>();

		if (emitter)
			OnSignalEmitterEnter?.Invoke(emitter);
	}

	private void OnTriggerExit(Collider other)
	{
		SignalEmitter emitter = other.gameObject.GetComponent<SignalEmitter>();

		if (emitter)
			OnSignalEmitterLeave?.Invoke(emitter);
	}
}
