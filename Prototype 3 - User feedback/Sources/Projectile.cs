using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[ReadOnly] public PlayerAttackData Data;

	private Transform _child;
	private EntityIdentity _caster;
	private float _startTime;
	private SpriteRenderer _renderer;
	private Vector3 _startOffset;
	private Vector3 _startSize;
	private Vector3 _startVelocity;

	private void Awake()
	{
		_child = transform.GetChild(0);
		_renderer = GetComponentInChildren<SpriteRenderer>();
	}

	public void SetWeaponData(Vector3 startOffset, Vector3 startScale, Vector3 startVelocity)
	{
		_startOffset = startOffset;
		_startSize = startScale;
		_startVelocity = startVelocity;
	}

	public void Init(EntityIdentity caster)
	{
		_caster = caster;
		_startTime = 0;
		_renderer.flipX = Data.Flip;
		_renderer.sprite = null;
	}

	private void Update()
	{
		_startTime += Time.deltaTime;
		Evaluate(_startTime);
		if (_startTime >= Data.Lifetime)
			gameObject.SetActive(false);
	}

	private Vector3 GetKnockbackDirection(Vector3 hitPos)
	{
		switch (Data.KbDirection)
		{
			case PlayerAttackData.KnockbackDirection.FROM_CENTER:
				return (GetComponent<SphereCollider>().center.WithY(hitPos.y) - hitPos).normalized;
			case PlayerAttackData.KnockbackDirection.RIGHT:
				return transform.right.WithY(0);
			case PlayerAttackData.KnockbackDirection.LEFT:
				return -transform.right.WithY(0);
			case PlayerAttackData.KnockbackDirection.FORWARD:
				return transform.forward.WithY(0);
			case PlayerAttackData.KnockbackDirection.BACKWARD:
				return -transform.forward.WithY(0);
		}
		return Vector3.zero;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _caster.gameObject) return;

		IDamageProcessor damageProcessor = other.GetComponent<IDamageProcessor>();

		if (damageProcessor != null)
		{
			Vector3 direction = GetKnockbackDirection(other.transform.position);
			float knockbackForce = Data.BaseKnockbackForce;

			damageProcessor.ApplyKnockback(gameObject, direction * knockbackForce);
		}
	}

	private void Evaluate(float timeline)
	{
		float ratio = timeline / Data.Lifetime;
		float timePerFrame = Data.Lifetime / (Data.Animations.Length);
		int frame = Mathf.FloorToInt(timeline / timePerFrame);

		if (timeline >= Data.Lifetime || frame >= Data.Animations.Length)
			_renderer.sprite = null;
		else
			_renderer.sprite = Data.Animations[frame];
		if (Data.FollowPlayer)
			transform.position = _caster.transform.position;
		_child.localScale = _startSize * Data.ScaleFactor.Evaluate(ratio);
		_child.localPosition = _startOffset + (_startVelocity * Data.VelocityFactor.Evaluate(ratio));
		_renderer.color = Data.Gradient.Evaluate(ratio);
		GetComponent<BoxCollider>().center = _child.localPosition;
		GetComponent<SphereCollider>().center = _child.localPosition;
	}
}
