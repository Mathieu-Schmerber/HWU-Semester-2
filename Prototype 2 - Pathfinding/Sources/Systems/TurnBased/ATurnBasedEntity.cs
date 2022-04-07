using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines an entity that can play turn-based
/// </summary>
public abstract class ATurnBasedEntity : EntityIdentity
{
	#region Fields

	[SerializeField] private GameObject _playIndicatorPrefab;

	private Animator	 _playIndicatorInstance;
	private bool _hasExecutedAction;

	#endregion

	#region Properties

	public static event Action<ATurnBasedEntity> OnTurnActionExecuting;
	public static event Action<ATurnBasedEntity> OnTurnBegan;
	public static event Action<ATurnBasedEntity> OnTurnEnded;

	public bool HasExecutedAnAction {
		get => _hasExecutedAction; 
		protected set {
			_hasExecutedAction = value;
			if (_hasExecutedAction)
				OnTurnActionExecuting?.Invoke(this);
	}}

	public int Id { get; set; }
	public bool IsBusy { get; private set; }
	public bool IsPlaying { get => TurnBasedManager.Instance.CurrentlyPlayingEntity == this; }
	public bool ReadyToStart { get; private set; }

	public abstract TeamData Team { get; }

	#endregion

	/// <summary>
	/// Spawns the play indicator effect
	/// </summary>
	private void Start()
	{
		Instantiate(_playIndicatorPrefab, transform).GetComponent<Animator>();
		ReadyToStart = true;
	}

	/// <summary>
	/// Called by the TurnBasedManager when the entity can start playing
	/// </summary>
	public virtual void OnTurnBegin()
	{
		OnTurnBegan?.Invoke(this);
	}

	/// <summary>
	/// Called by the TurnBasedManager when the entity must stop playing
	/// </summary>
	public virtual void OnTurnEnd()
	{
		OnTurnEnded?.Invoke(this);
		if (MovementPoints != null)
			MovementPoints.Value = MovementPoints.Max;
		HasExecutedAnAction = false;
	}

	public void SetBusy() => IsBusy = true;

	public void SetBusy(float time)
	{
		IsBusy = true;
		Invoke(nameof(SetFree), time);
	}

	public void SetFree()
	{
		IsBusy = false;
		OnAvailable();
	}

	protected abstract void OnAvailable();

	public override void OnDeath(GameObject attacker, IDamageProcessor victim)
	{
		base.OnDeath(attacker, victim);
		TurnBasedManager.Instance.RemoveEntity(this);
	}

	#region Tools

	/// <summary>
	/// Rotate the entity's graphics towards a node smoothly
	/// </summary>
	/// <param name="target">The node to loot at</param>
	/// <param name="graphics">The entity's graphics</param>
	/// <param name="eulerOffset">An offset if needed</param>
	/// <param name="duration"></param>
	/// <param name="onStart"></param>
	/// <param name="onDone"></param>
	/// <returns></returns>
	public IEnumerator LookAtNodeOnDuration(Node target, Transform graphics, Vector3 eulerOffset, float duration, Action onStart = null, Action onDone = null)
	{
		Vector3 direction = (target.WorldPos - transform.position).normalized;
		Quaternion startRotation = graphics.rotation;
		Quaternion endRotation = Quaternion.Euler(Quaternion.LookRotation(direction, Vector3.up).eulerAngles - eulerOffset);
		float time = 0;

		onStart?.Invoke();
		while (time < duration)
		{
			graphics.rotation = Quaternion.Lerp(startRotation, endRotation, time / duration);
			time += Time.deltaTime;
			yield return null;
		}
		onDone?.Invoke();
	}

	/// <summary>
	/// Rotate the entity's graphics towards a node smoothly
	/// </summary>
	/// <param name="target">The node to loot at</param>
	/// <param name="graphics">The entity's graphics</param>
	/// <param name="eulerOffset">An offset if needed</param>
	/// <param name="_360angleDuration">The time the entiy would take to make a full 360 turn</param>
	/// <param name="onStart">Callback before turning onStart(float duration, float angle)</param>
	/// <param name="onDone"></param>
	/// <returns></returns>
	public IEnumerator LookAtNode(Node target, Transform graphics, Vector3 eulerOffset, float _360angleDuration, Action<float, float> onStart = null, Action onDone = null)
	{
		Vector3 direction = (target.WorldPos - transform.position).normalized;
		Quaternion startRotation = graphics.rotation;
		Quaternion endRotation = Quaternion.Euler(Quaternion.LookRotation(direction, Vector3.up).eulerAngles - eulerOffset);

		float angle = Quaternion.Angle(startRotation, endRotation);
		float duration = (angle / 360f) * _360angleDuration;
		float time = 0;

		onStart?.Invoke(duration, angle);
		while (time < duration)
		{
			graphics.rotation = Quaternion.Lerp(startRotation, endRotation, time / duration);
			time += Time.deltaTime;
			yield return null;
		}
		onDone?.Invoke();
	}

	#endregion
}