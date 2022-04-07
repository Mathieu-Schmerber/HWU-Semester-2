using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("Zoom")]
	[SerializeField] private float _minSize;
	[SerializeField] private float _maxSize;
	[SerializeField] private float _sensitivity = 10;

	[Header("Focus")] 
	[SerializeField] private Vector3 _offset;
	[SerializeField] private float _travelTime = 0.2f;

	private Camera _camera;

	private Vector3 _origin;
	private Vector3 _diff;
	private Vector3 _reset;
	private bool _drag = false;
	private bool _allowDrag = true;

	private bool _isLocked = false;
	private float _cameraDistance;

	#region Unity builtins

	private void OnEnable()
	{
		ATurnBasedEntity.OnTurnBegan += ATurnBasedEntity_OnTurnBegan;
	}

	private void OnDisable()
	{
		ATurnBasedEntity.OnTurnBegan -= ATurnBasedEntity_OnTurnBegan;
	}

	private void Awake()
	{
		_camera = GetComponent<Camera>();
		_reset = _camera.transform.position;
		_origin = _reset;
		_cameraDistance = Vector3.Distance(_camera.transform.position, Vector3.zero);
	}

	#endregion

	public void Lock()
	{
		Tween.Value(_camera.orthographicSize, _minSize, (value) => _camera.orthographicSize = value, 0.123f, 0);
		_isLocked = true;
	}

	private void AllowDrag()
	{
		_allowDrag = true;
		_origin = _camera.transform.position;
		_diff = Vector3.zero;
	}

	private Vector3 GetFocusPoint()
	{
		Plane logicalPlane = new Plane(Vector3.up, Vector3.zero);
		Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
		float distance = 0;

		logicalPlane.Raycast(ray, out distance);
		return ray.GetPoint(distance); //_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, _cameraDistance));
	}

	public void Focus(GameObject gameObject)
	{
		Tween.Stop(_camera.transform.GetInstanceID(), Tween.TweenType.Position);
		_allowDrag = false;

		Vector3 focusPoint = GetFocusPoint();
		Vector3 dir = gameObject.transform.position.WithY(focusPoint.y) - focusPoint;

		Tween.Position(_camera.transform, _camera.transform.position + dir + _offset, _travelTime, 0, Tween.EaseOut, completeCallback: () => AllowDrag());
	}

	private void ATurnBasedEntity_OnTurnBegan(ATurnBasedEntity entity)
	{
		if (!_isLocked)
			Focus(entity.gameObject);
	}

	private void HandleZoom()
	{
		if (_isLocked) return;
		float ortho = _camera.orthographicSize;
		ortho -= Input.GetAxis("Mouse ScrollWheel") * _sensitivity;
		ortho = Mathf.Clamp(ortho, _minSize, _maxSize);

		if (Input.GetAxis("Mouse ScrollWheel") != 0)
			Tween.Value(_camera.orthographicSize, ortho, (value) => _camera.orthographicSize = value, 0.123f, 0);
	}

	private void HandleDrag()
	{
		if (Input.GetMouseButton(1))
		{
			_diff = _camera.ScreenToWorldPoint(Input.mousePosition) - _camera.transform.position;
			if (!_drag)
			{
				_drag = true;
				_origin = _camera.ScreenToWorldPoint(Input.mousePosition);
			}
		}
		else
			_drag = false;

		Tween.Position(_camera.transform, _origin - _diff, 0.1f, 0);
	}

	private void LateUpdate()
	{
		HandleZoom();
		if (_allowDrag && !_isLocked)
			HandleDrag();
	}
}
