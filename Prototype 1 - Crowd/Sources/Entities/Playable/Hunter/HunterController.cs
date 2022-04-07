using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HunterController : HunterEmitter
{
	private InputManager _input;
	private Rigidbody _rb;

	[SerializeField] private float speed;
	[SerializeField] private LayerMask _groundLayer;

	private Vector2 ConvertInputs(Vector2 input) => input.InverseXY() * new Vector2(-1, 1);

	protected override sealed void Awake()
	{
		base.Awake();

		_input = GetComponent<InputManager>();
		_rb = GetComponent<Rigidbody>();
	}

	protected override void Update()
	{
		base.Update();

		if (_shoot.CanMove)
		{
			Vector3 movement = ConvertInputs(_input.MovementAxis).ToVector3XZ();

			RotateTowardsDirection(_rb.position + movement);
			_animator.SetFloat("Speed", movement.magnitude);
		}
		if (_input.IsShootPressed)
			AimTowardMouse(shoot: true);
		else if (!_shoot.CanMove)
			AimTowardMouse(shoot: false);
	}

	private void AimTowardMouse(bool shoot)
	{
		_shoot.AimAt(_shoot.Crosshair.position.WithY(transform.position.y), shoot);
	}

	public void RotateTowardsDirection(Vector3 position)
	{
		if (position == _rb.position) return;

		Vector3 dir = (position - transform.position).normalized;
		Quaternion target = Quaternion.LookRotation(dir, transform.up);

		transform.rotation = Quaternion.Lerp(transform.rotation, target, 0.125f);
	}

	private void FixedUpdate()
	{
		if (!_shoot.CanMove) return;

		Vector3 speedInput = ConvertInputs(_input.MovementAxis).normalized.ToVector3XZ() * speed;

		_rb.MovePosition(_rb.position + speedInput * Time.fixedDeltaTime);
	}
}
