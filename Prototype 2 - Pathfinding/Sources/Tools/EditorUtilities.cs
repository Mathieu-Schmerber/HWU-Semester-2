using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class EditorUtilities
{
	/// <summary>
	/// Get all animations of a given Animator, based on its AnimatorController
	/// </summary>
	/// <param name="animator"></param>
	/// <returns></returns>
	public static List<AnimationClip> GetAnimatorAnimations(Animator animator)
	{
		return animator.runtimeAnimatorController.animationClips.ToList();
	}
}