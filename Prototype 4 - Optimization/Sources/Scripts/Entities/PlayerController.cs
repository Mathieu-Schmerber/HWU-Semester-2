using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public enum Reactor { LEFT, RIGHT}

    [SerializeField] private TrailRenderer _leftReactor;
    [SerializeField] private TrailRenderer _rightReactor;

    private Dictionary<Reactor, bool> _reactors = new Dictionary<Reactor, bool>() { 
		{ Reactor.LEFT, false }, { Reactor.RIGHT, false } 
	};

    private float _turnSpeed;
    private float _maxSpeed;
    [SerializeField] private float _velocityDrag = 1;
    [SerializeField] private float _rotationDrag = 1;

    private float _scaledTurnDrag = 1;
    private float _rotationVelocity;
    private Vector3 _velocity;
    private EntityIdentity _identity;
    private IDamageProcessor _processor;

    public Vector3 Velocity => _velocity;

	private void OnEnable()
	{
		GameLoop.OnGameEnded += GameEnd;	
	}

	private void OnDisable()
	{
        GameLoop.OnGameEnded -= GameEnd;
    }

    private void Awake()
	{
        _processor = GetComponent<IDamageProcessor>();
        _identity = GetComponent<EntityIdentity>();
    }

	private void Update()
    {
        if (!_reactors[Reactor.LEFT] && !_reactors[Reactor.RIGHT] || _processor.IsDead) 
            return;

        _maxSpeed = _identity.Speed.Value;
        _velocity += transform.up * _maxSpeed * Time.deltaTime;

        if (_reactors[Reactor.LEFT] && _reactors[Reactor.RIGHT]) 
            return;

        _scaledTurnDrag = _identity.Maniability.Value / 110;
        _turnSpeed = _identity.Maniability.Value * _scaledTurnDrag;
        float zTurnAcceleration = (_reactors[Reactor.LEFT] ? 1 : -1) * _turnSpeed;
        _rotationVelocity += zTurnAcceleration * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_processor.IsDead) return;

        _velocity = _velocity * (1 - Time.deltaTime * _velocityDrag);
        _velocity = Vector2.ClampMagnitude(_velocity, _maxSpeed);

        _rotationVelocity = _rotationVelocity * (1 - Time.deltaTime * _rotationDrag);
        _rotationVelocity = Mathf.Clamp(_rotationVelocity, -_turnSpeed, _turnSpeed);

        // update transform
        transform.position += _velocity * Time.deltaTime;
        transform.Rotate(0, 0, _rotationVelocity * Time.deltaTime);
    }

    public void TurnOn(Reactor reactor)
	{
		_reactors[reactor] = true;
        _leftReactor.emitting = _reactors[Reactor.LEFT];
        _rightReactor.emitting = _reactors[Reactor.RIGHT];
    }

    public void TurnOff(Reactor reactor)
	{
		_reactors[reactor] = false;
        _leftReactor.emitting = _reactors[Reactor.LEFT];
        _rightReactor.emitting = _reactors[Reactor.RIGHT];
    }

    private void GameEnd()
	{
        TurnOff(Reactor.LEFT);
        TurnOff(Reactor.RIGHT);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        _velocity = Vector3.zero;
        _rotationVelocity = 0;
    }
}
