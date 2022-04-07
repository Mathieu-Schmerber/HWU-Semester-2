using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greatsword : Weapon
{
	public override void OnAnimationEvent(string animation)
	{
		if (animation == "Attack")
		{
			if (CurrentWeaponAttack != Data.AttackCombos.Last())
			{
				_controller.CanMove = false;
				_controller.IsAimLocked = true;
			}
		}
		base.OnAnimationEvent(animation);
	}

	public override void OnAnimationExit(AnimatorStateInfo stateInfo)
	{
		_controller.CanMove = true;
		_controller.IsAimLocked = false;
	}
}
