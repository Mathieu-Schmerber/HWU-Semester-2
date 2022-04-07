using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	public enum KnockbackDirection
	{
		FROM_CENTER,
		FORWARD
	}

	[SerializeField] private GameObject _onHitFx;
	[SerializeField] private float _hitYRotation;
	[SerializeField] private float _activeTime;
	[SerializeField] private int _baseDamage;
	[SerializeField] private int _baseKnockbackForce;
	[SerializeField] private KnockbackDirection _knockbackDirection;
	public bool FollowCaster;
	public float Range;

	private List<Collider> _hitColliders = new List<Collider>();
	private float _startTime;
	private Vector3 _baseOffset;
	private Vector3 _velocity;
	private EntityIdentity _caster;

	public void Init(EntityIdentity caster)
	{
		_caster = caster;
		_startTime = Time.time;
		_hitColliders.Clear();
	}

	public void OnStart(Vector3 offset, Vector3 travelDistance)
	{
		_baseOffset = offset;

		Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);
		localOffsetDir.x *= -1;
		transform.position = _caster.transform.position + localOffsetDir;

		Vector3 localTravelDir = transform.InverseTransformDirection(travelDistance);
		localTravelDir.x *= -1;
		_velocity = localTravelDir / _activeTime;
	}

	private void Update()
	{
		if (Time.time - _startTime >= _activeTime)
			gameObject.SetActive(false);
		else if (FollowCaster)
		{
			Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);

			localOffsetDir.x *= -1;
			transform.position = _caster.transform.position + localOffsetDir;
		}
		transform.position += _velocity * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _caster.gameObject || _hitColliders.Contains(other)) return;

		IDamageProcessor damageProcessor = other.GetComponent<IDamageProcessor>();

		_hitColliders.Add(other);
		if (damageProcessor != null)
		{
			Vector3 direction = _knockbackDirection == KnockbackDirection.FORWARD ? transform.forward : (other.transform.position - transform.position).normalized.WithY(0);
			float knockbackForce = _baseKnockbackForce;

			// TODO: apply _caster stats
			damageProcessor.ApplyKnockback(gameObject, direction * knockbackForce);
			damageProcessor.ApplyDamage(gameObject, _baseDamage);
			Instantiate(_onHitFx, other.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y + _hitYRotation, 0));
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f - transform.forward * 1.75f, new Vector3(0.4f, 2, .4f));

		Gizmos.color = Color.white;
		Gizmos.DrawRay(transform.position - transform.forward * 1.75f, transform.forward * Range);
	}
}
