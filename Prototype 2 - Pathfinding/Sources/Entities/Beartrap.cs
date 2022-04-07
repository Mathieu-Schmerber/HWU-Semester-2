using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beartrap : EntityIdentity
{
	[SerializeField, ValueDropdown(nameof(GetAnimatorAnimations))] private AnimationClip _triggerAnimation;

	private Animator _animator;
	private DissolveEffect _dissolveEffect;

	protected override void Awake()
	{
		base.Awake();
		_animator = GetComponentInChildren<Animator>();
		_dissolveEffect = GetComponent<DissolveEffect>();
	}

	public override void OnEntityCross(ATurnBasedEntity entity)
	{
		_animator.Play(_triggerAnimation.name);

		Tween.Stop(entity.transform.GetInstanceID());
		entity.MovementPoints.Value = 0;
		entity.GetComponent<IDamageProcessor>()?.ApplyDamage(gameObject, this.Damage.Value);
		entity.SetFree();

		_dissolveEffect.Invoke(nameof(_dissolveEffect.FadeOut), _triggerAnimation.length);
		Destroy(gameObject, _triggerAnimation.length + _dissolveEffect._deathDuration);
		EntityMap.Instance.RemoveEntity(this);
	}


	#region Editor

	private IEnumerable GetAnimatorAnimations()
	{
		Animator animator = gameObject.GetComponentInChildren<Animator>();
		return EditorUtilities.GetAnimatorAnimations(animator);
	}

	#endregion
}
