using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[SerializeField] private int _baseDamage;
	[SerializeField] private float _baseKnockbackForce;
	[SerializeField] private float _activeTime;
	[SerializeField] private float _baseRange;
	[SerializeField] private GameObject _onHitFx;

	private AController _controller;
	private float _startTime;

	public float Range => _baseRange;

	private void Awake()
	{
		_controller = GetComponentInParent<AController>();
	}

	private void OnEnable()
	{
		_startTime = Time.time;
		transform.rotation = Quaternion.LookRotation(_controller.GetAimNormal());
	}

	private void Update()
	{
		if (Time.time - _startTime >= _activeTime)
			gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		// TODO: create a clean target layering system
		if (other.gameObject == transform.parent.gameObject || other.gameObject.GetComponent<PlayerController>() == null) return;

		IDamageProcessor damageProcessor = other.GetComponent<IDamageProcessor>();

		if (damageProcessor != null)
		{
			Vector3 direction = transform.forward;
			float knockbackForce = _baseKnockbackForce;

			// TODO: apply _caster stats
			damageProcessor.ApplyKnockback(gameObject, direction * knockbackForce);
			damageProcessor.ApplyDamage(gameObject, _baseDamage);
			InputHandler.VibrateController(0.123f, 0.2f);
			if (_onHitFx)
				Instantiate(_onHitFx, other.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
		}
	}
}