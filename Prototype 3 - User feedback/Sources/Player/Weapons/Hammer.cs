using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hammer : Weapon
{
	private CameraController _camera;

	protected void Awake()
	{
		_camera = GameObject.FindObjectOfType<CameraController>();
	}

	public override void OnAnimationEvent(string animation)
	{
		if (animation == "Attack")
		{
			if (CurrentWeaponAttack == Data.AttackCombos.Last())
			{
				_controller.IsAimLocked = true;
				_camera.Shake(Vector3.one * 0.5f, 0.2f);
				InputHandler.VibrateController(0.246f, 0.2f);
			}
		}
		base.OnAnimationEvent(animation);
	}

	public override void OnAnimationEnter(AnimatorStateInfo stateInfo)
	{
		_controller.CanMove = false;
	}

	public override void OnAnimationExit(AnimatorStateInfo stateInfo)
	{
		_controller.CanMove = true;
		_controller.IsAimLocked = false;
	}
}
