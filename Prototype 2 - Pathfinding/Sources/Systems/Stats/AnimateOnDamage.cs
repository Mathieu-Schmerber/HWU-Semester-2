using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Plays selected animations OnDamage and OnDeath
/// </summary>
public class AnimateOnDamage : MonoBehaviour, IDamageableListener
{
	#region Fields

	// Hit animation
	[ToggleGroup(nameof(_enableHitAnim), "Animation on hit")] 
	public bool _enableHitAnim = true;
	[SerializeField, ToggleGroup(nameof(_enableHitAnim)), ValueDropdown(nameof(GetAnimatorAnimations))]
	private AnimationClip _hitAnim;

	// Death animation
	[ToggleGroup(nameof(_enableDeathAnim), "Animation on death")]
	public bool _enableDeathAnim = true;
	[SerializeField, ToggleGroup(nameof(_enableDeathAnim)), ValueDropdown(nameof(GetAnimatorAnimations))] 
	private AnimationClip _deathAnim;

	#endregion

	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	public void OnDamageDealt(GameObject attacker, IDamageProcessor victim, int amount)
	{
		if (_enableHitAnim && !victim.IsDead)
			_animator.Play(_hitAnim.name);
	}

	public void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		if (_enableDeathAnim)
			_animator.Play(_deathAnim.name);
	}

	#region Editor

	private IEnumerable GetAnimatorAnimations()
	{
		Animator animator = gameObject.GetComponentInChildren<Animator>();
		return EditorUtilities.GetAnimatorAnimations(animator);
	}

	#endregion
}