using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goblin : EnemyAI, IAnimationListener
{
	[SerializeField, ValueDropdown(nameof(GetAnimatorAnimations))] private AnimationClip _attackAnimation;

	private Node _currentTarget;

	protected override void OnAttack(Node node)
	{
		if (!IsPlaying || node.IsEmpty) return;

		_currentTarget = node;
		SetBusy(_attackAnimation.length + 0.3f);
		StartCoroutine(LookAtNodeOnDuration(node, transform.GetChild(0), Vector3.zero, 0.3f, onDone: () => _animator.Play(_attackAnimation.name)));
	}

	public override void OnTurnBegin()
	{
		base.OnTurnBegin();
	}

	private void ApplyDamage()
	{
		foreach (EntityType entity in _currentTarget.Entities.ToList())
			entity.GetComponent<IDamageProcessor>()?.ApplyDamage(gameObject, Damage.Value);
	}

	public void OnAnimationEvent(string animationEvent)
	{
		if (animationEvent == "Attack")
			ApplyDamage();
	}

	#region Editor

	private IEnumerable GetAnimatorAnimations()
	{
		Animator animator = gameObject.GetComponentInChildren<Animator>();
		return EditorUtilities.GetAnimatorAnimations(animator);
	}

	#endregion
}
