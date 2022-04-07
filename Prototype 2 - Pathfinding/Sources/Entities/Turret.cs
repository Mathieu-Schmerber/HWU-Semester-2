using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the Turret's logic
/// </summary>
public class Turret : PlayableEntity
{
	#region Fields

	[SerializeField, ValueDropdown(nameof(GetAnimatorAnimations))] private AnimationClip _shootAnimation;
	[SerializeField] private float _360AngleDuration;

	private Transform _gfx;
	private Animator _animator;
	private LineRenderer _shotFxLine;
	private Vector3 _gfxEulerOffset;

	#endregion

	protected override void Awake()
	{
		base.Awake();
		_animator = GetComponentInChildren<Animator>();
		_gfx = _animator.transform;
		_shotFxLine = GetComponentInChildren<LineRenderer>(includeInactive: true);

		_gfxEulerOffset = transform.rotation.eulerAngles - _gfx.rotation.eulerAngles;
	}

	protected override void Attack(Node node)
	{
		// Aim at target, and shoot it
		StartCoroutine(LookAtNode(node, _gfx, _gfxEulerOffset, _360AngleDuration,
			onStart: (duration, angle) =>
			{
				SetBusy(duration + _shootAnimation.length);
				_animator.SetFloat("Angle", angle);
			},
			onDone: () => {
				_animator.SetFloat("Angle", 0);
				StartCoroutine(PlayShootAnimation(node));
			}
		));
	}

	/// <summary>
	/// Applies damages to the target and play the shoot animation
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	private IEnumerator PlayShootAnimation(Node target)
	{
		// Applying damage
		if (!target.IsEmpty)
		{
			// calling ToList() to create a copy since an entity my get removed after dying
			foreach (EntityType entity in target.Entities.ToList())
				entity.GetComponent<IDamageProcessor>()?.ApplyDamage(gameObject, Damage.Value);
		}

		// Shoot animation
		_animator.Play(_shootAnimation.name);
		
		// Displaying the shot effect
		_shotFxLine.SetPositions(new Vector3[]{
			_shotFxLine.transform.position, 
			target.WorldPos.WithY(_shotFxLine.transform.position.y
		)});
		_shotFxLine.enabled = true;
		yield return new WaitForSeconds(0.1f);
		_shotFxLine.enabled = false;
	}

	#region Editor

	private IEnumerable GetAnimatorAnimations()
	{
		Animator animator = gameObject.GetComponentInChildren<Animator>();
		return EditorUtilities.GetAnimatorAnimations(animator);
	}

	#endregion
}
