using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Gradient _shieldGradient;

	private SpriteRenderer _renderer;
    private EntityIdentity _identity;

	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
		_identity = GetComponentInParent<EntityIdentity>();
	}

	private void OnEnable()
	{
		_identity.OnCurrentHealthValueChanged += _identity_OnCurrentHealthValueChanged;
	}

	private void OnDisable()
	{
		_identity.OnCurrentHealthValueChanged += _identity_OnCurrentHealthValueChanged;
	}

	private void _identity_OnCurrentHealthValueChanged(float value)
	{
		float ratio = 1f / _identity.MaxHealth.Value;
		_renderer.color = _shieldGradient.Evaluate(-ratio * value + 1);
	}
}
