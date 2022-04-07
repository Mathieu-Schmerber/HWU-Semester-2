using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class PlayerAttackData : ScriptableObject
{
	public enum KnockbackDirection
	{
		FROM_CENTER,
		RIGHT,
		LEFT,
		FORWARD,
		BACKWARD
	}

	public enum ColliderType
	{
		BOX,
		SPHERE
	}

	public string Name;
	public bool FollowPlayer;

	[BoxGroup("Animation")]
	public Sprite[] Animations;
	[BoxGroup("Animation")]
	public float Lifetime;
	[BoxGroup("Animation")]
	public bool Flip;
	[BoxGroup("Animation")]
	public Gradient Gradient;
	[BoxGroup("Animation")]
	public AnimationCurve ScaleFactor;
	[BoxGroup("Animation")]
	public AnimationCurve VelocityFactor;

	[BoxGroup("Collider")]
	public ColliderType Collider;
	[BoxGroup("Collider")]
	public Vector3 Center;
	[BoxGroup("Collider")]
	public Vector3 Size;

	public KnockbackDirection KbDirection;
	public float BaseKnockbackForce;

	[Button]
	private void Spawn()
	{
		var go = Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileTemplate"));
		TranslateTo(go);
	}

	public void TranslateTo(GameObject go)
	{
		Projectile proj = go.GetComponent<Projectile>();
		var box = go.GetComponent<BoxCollider>();
		var sphere = go.GetComponent<SphereCollider>();

		proj.Data = this;
		box.enabled = false;
		sphere.enabled = false;
		switch (Collider)
		{
			case ColliderType.BOX:
				box.size = Size;
				box.center = Center;
				box.enabled = true;
				break;
			case ColliderType.SPHERE:
				sphere.center = Center;
				sphere.radius = Size.x;
				sphere.enabled = true;
				break;
		}
	}
}