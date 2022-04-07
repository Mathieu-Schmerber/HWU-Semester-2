using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfController : WolfEmitter
{
	private InputManager _input;
	private Rigidbody _rb;

	[SerializeField] private float speed;

	protected override sealed void Awake()
	{
		_input = GetComponent<InputManager>();
		_rb = GetComponent<Rigidbody>();
		base.Awake();
	}

	private Vector2 ConvertInputs(Vector2 input) => input.InverseXY() * new Vector2(-1, 1);

	protected override void Update()
	{
		base.Update();

		Vector3 movement = ConvertInputs(_input.MovementAxis).ToVector3XZ();

		RotateTowardsDirection(_rb.position + movement);
		_animator.SetFloat("Speed", movement.magnitude);
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
		Vector3 speedInput = ConvertInputs(_input.MovementAxis).normalized.ToVector3XZ() * speed;

		_rb.MovePosition(_rb.position + speedInput * Time.fixedDeltaTime);
	}
}
