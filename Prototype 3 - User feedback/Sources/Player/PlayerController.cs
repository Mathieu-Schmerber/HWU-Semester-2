using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerController : AController
{
	#region Properties

	[SerializeField] private Cooldown _dashCooldown;
	[SerializeField] private float _dashDistance;
	[SerializeField] private float _dashTime;
	[SerializeField] private int _afterImagesNumber;
	[SerializeField] private ParticleSystem _dashFx;

	private InputHandler _inputs;

	#endregion

	#region Unity builtins

	private void OnEnable()
	{
		_inputs.OnDashPressed += OnDashInput;
	}

	private void OnDisable()
	{
		_inputs.OnDashPressed -= OnDashInput;
	}

	protected override void Awake()
	{
		base.Awake();
		_inputs = GetComponent<InputHandler>();
		_dashCooldown.Init();
	}

	#endregion

	private void OnDashInput()
	{
		if (_dashCooldown.IsOver())
		{
			Vector3 direction = GetMovementNormal().magnitude > 0 ? GetMovementNormal() : GetAimNormal();

			_dashFx.Play(true);
			Dash(direction, _dashDistance, _dashTime, _afterImagesNumber);
			_dashCooldown.Reset();
		}
	}

	#region Abstraction

	protected override Vector3 GetMovementsInputs() => _inputs.MovementAxis.ToVector3XZ().ToIsometric();

	protected override Vector3 GetTargetPosition()
	{
		Vector3 aimInput = GetMovementsInputs();
		bool isAiming = aimInput.magnitude > 0;

		if (IsAimLocked)
			return _rb.position + _graphics.transform.forward;
		else if (isAiming)
			return _rb.position + aimInput;
		return _rb.position + _graphics.transform.forward;
	}

	#endregion
}