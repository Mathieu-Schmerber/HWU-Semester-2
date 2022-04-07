using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterShoot : MonoBehaviour, IAnimationReceiver
{
	private Animator _animator;
	private bool _canMove;

	private AudioSource _source;
	private LineRenderer _shootLineEffect;

	protected Transform _crosshair;
	[SerializeField] private GameObject _crosshairPrefab;
	[SerializeField] private GameObject _hitFx;
	[SerializeField] private Vector3 _gunEnd;
	[SerializeField] private int _gunDamage;

	public Transform Crosshair => _crosshair;
	public bool CanMove => _canMove;

	protected virtual void Awake()
	{
		_source = GetComponent<AudioSource>();
		_shootLineEffect = GetComponent<LineRenderer>();
		_animator = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		_crosshair = Instantiate(_crosshairPrefab).transform;
		_crosshair.GetComponent<LinkToObject>().ObjectLink = transform;

		_shootLineEffect.enabled = false;
	}

	private void OnDestroy()
	{
		if (_crosshair != null)
			Destroy(_crosshair.gameObject);
	}

	public void AimAt(Vector3 position, bool shoot = true)
	{
		if (shoot)
			_animator.SetBool("Shoot", true);

		Vector3 dir = (position - transform.position).normalized;
		Quaternion target = Quaternion.LookRotation(dir, transform.up);

		transform.rotation = Quaternion.Lerp(transform.rotation, target, 0.125f);
	}

	public virtual void OnShootAnimationEnd()
	{
		_animator.SetBool("Shoot", false);
	}

	public virtual void OnShootAnimationShoot()
	{
		Ray ray = Gamemode.Instance.CurrentMode == Gamemode.Mode.HUNTER ?
			new Ray(transform.position, (_crosshair.position.WithY(transform.position.y) - transform.position).normalized) :
			new Ray(transform.position, transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			StartCoroutine(nameof(PlayShootEffect), hit.point);
			hit.transform.GetComponent<ADamageable>()?.Damage(_gunDamage);
		}
	}

	private IEnumerator PlayShootEffect(Vector3 destination)
	{
		_source.Play();
		_shootLineEffect.SetPositions(new Vector3[2] {
				destination + _gunEnd,
				transform.position + _gunEnd
		});
		Instantiate(_hitFx, destination + Vector3.up * _gunEnd.y, Quaternion.LookRotation(-transform.forward, transform.up));
		_shootLineEffect.enabled = true;
		yield return new WaitForSeconds(0.1f);
		_shootLineEffect.enabled = false;
	}

	public void OnAnimationEvent(string eventName)
	{
		switch (eventName)
		{
			case "OnShootAnimationEnd":
				OnShootAnimationEnd();
				break;
			case "OnShootAnimationShoot":
				OnShootAnimationShoot();
				break;
			case "OnLocomotionEnter":
				_canMove = true;
				break;
			case "OnLocomotionExit":
				_canMove = false;
				break;
			default:
				break;
		}
	}
}
