using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
	[SerializeField] private Vector3 _offset;

	private float _baseFov;
	private Camera _cam;

	public float BaseFov => _baseFov;

	private void Awake()
	{
		_cam = GetComponent<Camera>();
		_baseFov = _cam.fieldOfView;
	}

	private void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, _target.position + _offset, 0.123f);
	}

	public void Zoom(float amount)
	{
		_cam.fieldOfView = amount;
	}

	public void ResetZoom() => _cam.fieldOfView = _baseFov;

	public void Shake(Vector3 intensity, float duration)
	{
		Tween.Shake(transform, transform.position, intensity, duration, 0);
	}
}