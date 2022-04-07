using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kamikaze : EnemyAI, IAnimationListener
{
	[SerializeField, ValueDropdown(nameof(GetAnimatorAnimations))] private AnimationClip _attackAnimation;

	protected override void OnAttack(Node node)
	{
		_animator.Play(_attackAnimation.name, 0);
		SetBusy(_attackAnimation.length);
	}

	private void Explode()
	{
		foreach (Node node in CurrentNode.Neighbors)
		{
			foreach (EntityType entity in node.Entities.ToList())
				entity.GetComponent<IDamageProcessor>()?.ApplyDamage(gameObject, Damage.Value);
		}
		GetComponent<IDamageProcessor>().ApplyDamage(gameObject, 1000);
	}

	public void OnAnimationEvent(string animationEvent)
	{
		if (animationEvent == "Attack")
			Explode();
	}


	#region Editor

	private IEnumerable GetAnimatorAnimations()
	{
		Animator animator = gameObject.GetComponentInChildren<Animator>();
		return EditorUtilities.GetAnimatorAnimations(animator);
	}

	#endregion
}
