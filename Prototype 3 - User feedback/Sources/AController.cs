using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Pixelplacement;

[RequireComponent(typeof(EntityIdentity))]
public abstract class AController : SerializedMonoBehaviour
{
    #region Properties

    private AfterImageEffect _afterImageEffect;
    protected EntityIdentity _entity;
    protected Rigidbody _rb;
    protected Quaternion _desiredRotation;
    protected GameObject _graphics;
    protected Animator _gfxAnim;
    private Transform _target;

    private bool _canMove = true;
    [ShowInInspector] private float _rotationSpeed = 6f;

    public Transform Graphics => _graphics.transform;
    protected Transform _lockedTarget => _target;

    public bool CanMove { get => _canMove && !IsDashing; 
        set
        {
            if (value == false)
                _rb.velocity = Vector3.zero;
            _canMove = value;
        }
    }

	public bool IsDashing { get; private set; }

	public bool IsAimLocked { get; set; }

	#endregion

    #region Unity builtins

    // Get references
    protected virtual void Awake()
    {
        _entity = GetComponent<EntityIdentity>();
        _rb = GetComponent<Rigidbody>();
        _gfxAnim = GetComponentInChildren<Animator>();
        _afterImageEffect = GetComponentInChildren<AfterImageEffect>();
        _graphics = _gfxAnim.gameObject;
        _desiredRotation = transform.rotation;
    }

    // Initialization
    protected virtual void Update()
    {
        Vector3 dir = GetAimNormal();

        if (_graphics && !IsAimLocked)
            ApplySmoothRotation(_graphics.transform);

        Vector3 inputs = GetMovementsInputs();

        _gfxAnim?.SetFloat("Speed", inputs.magnitude);
        _gfxAnim?.SetFloat("Angle", Vector3.Angle(inputs, dir));
    }

    protected virtual void FixedUpdate() => Move();

    #endregion

    /// <summary>
	/// Gets the movement inputs
	/// </summary>
	/// <returns></returns>
	protected abstract Vector3 GetMovementsInputs();

    /// <summary>
    /// Gets the position of a target. <br/>
    /// Called if no target was locked.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector3 GetTargetPosition();

    public Vector3 GetAimNormal() => ((_target != null ? _target.position : GetTargetPosition()) - _rb.position).normalized.WithY(0);
    public Vector3 GetMovementNormal() => GetMovementsInputs().normalized;

    public void LockTarget(Transform target, bool forceRotation = false)
    {
        _target = target;
        if (forceRotation)
            _graphics.transform.rotation = Quaternion.LookRotation(GetAimNormal());
    }

    public void UnlockTarget() => _target = null;

    /// <summary>
    /// Rotation angle calculation and lerping
    /// </summary>
    protected void ApplySmoothRotation(Transform tr)
    {
        _desiredRotation = Quaternion.LookRotation(GetAimNormal());
        tr.rotation = Quaternion.Slerp(tr.transform.rotation, _desiredRotation, _rotationSpeed * Time.deltaTime);
        tr.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Movement speed calculation, and object motion
    /// </summary>
    protected virtual void Move()
    {
        if (!CanMove) return;

        // Calculate how fast we should be moving
        var targetVelocity = GetMovementsInputs();
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= _entity.Stats.Speed.Value;

        // Apply a force that attempts to reach our target velocity
        var velocity = _rb.velocity;
        var velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -_entity.Stats.Speed.Value, _entity.Stats.Speed.Value);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -_entity.Stats.Speed.Value, _entity.Stats.Speed.Value);
        velocityChange.y = 0;
        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void Dash(Vector3 direction, float distance, float time, int afterImages = 0)
    {
        Vector3 destination = transform.position + (direction * distance);

        if (afterImages > 0)
            _afterImageEffect.Play(time, afterImages);
        Tween.Position(transform, destination, time, 0, Tween.EaseOut,
            startCallback: () => IsDashing = true,
            completeCallback: () => IsDashing = false
        );
    }
}