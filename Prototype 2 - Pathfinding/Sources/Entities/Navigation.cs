using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigation : MonoBehaviour
{
	[SerializeField] private float _unitTravelDuration;
	[SerializeField] private Transform _gfx;

	private List<Node> _path;
	private ATurnBasedEntity _entity;
	private Animator _animator;

	private void Awake()
	{
		_entity = GetComponent<ATurnBasedEntity>();
		_animator = GetComponentInChildren<Animator>();
	}

	private void UpdateAnimatorSpeed(float value) => _animator.SetFloat("Speed", value);

	public void FollowPath(List<Node> path)
	{
		_path = path;
		_entity.SetBusy();
		for (int i = 0; i < _path.Count; i++)
		{
			Node lastNode = i == 0 ? null : _path[i - 1];
			Node node = _path[i];
			AnimationCurve curve = Tween.EaseLinear;
			float animationTarget = i == _path.Count - 1 ? 0 : 1;
			float animationCurrent = i == 0 ? 0 : 1;

			if (i == 0)
				curve = Tween.EaseIn;
			else if (i == _path.Count - 1)
				curve = Tween.EaseOut;
			Tween.Value(animationCurrent, animationTarget, UpdateAnimatorSpeed, _unitTravelDuration, _unitTravelDuration * i, curve);
			Tween.Position(transform, node.WorldPos, _unitTravelDuration, _unitTravelDuration * i, curve, completeCallback: () => NotifyNode(node));

			Vector3 origin = (lastNode == null ? transform.position : lastNode.WorldPos);
			Vector3 dir = (node.WorldPos - origin).normalized;
			Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);

			Tween.Rotation(_gfx, rotation, _unitTravelDuration, _unitTravelDuration * i);
		}
		_entity.MovementPoints.Value -= _path.Count - 1;
	}

	public void MoveToNode(Node destination)
	{
		var path = EntityMap.Instance.GetPath(_entity.CurrentNode, destination);

		if (path == null)
		{
			_entity.SetFree();
			return;
		}
		FollowPath(EntityMap.Instance.GetPath(_entity.CurrentNode, destination));
	}

	private void NotifyNode(Node node)
	{
		transform.position = node.WorldPos;
		_gfx.localPosition = Vector3.zero;
		EntityMap.Instance.MoveEntity(_entity.CurrentNode, node, _entity);

		if (!node.IsEmpty)
			node.Entities.ToList().ForEach(x => x.OnEntityCross(_entity));

		if (_path.Last() == node)
			_entity.SetFree();
	}
}