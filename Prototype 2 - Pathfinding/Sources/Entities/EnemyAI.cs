using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Defines a turn based enemy
/// </summary>
public abstract class EnemyAI : ATurnBasedEntity
{
	#region Types

	public enum Action
	{
		MOVE,
		ATTACK
	}

	public struct NodeValue
	{
		public Node Source;
		public int Value;

		public NodeValue(Node source, int value)
		{
			Source = source;
			Value = value;
		}
	}

	public struct ActionNodePair
	{
		public Action Action;
		public Node Node;

		public ActionNodePair(Action action, Node node)
		{
			Action = action;
			Node = node;
		}
	}

	#endregion

	#region Fields

	[SerializeField] private TargetTableData _targetWorth;

	private int _beginTurnHealth;
	private IDamageProcessor _damageable;
	private Navigation _navigation;
	private TeamData _team;
	protected Queue<ActionNodePair> _actionQueue = new Queue<ActionNodePair>();
	protected Dictionary<Node, List<NodeValue>> _costMap = new Dictionary<Node, List<NodeValue>>();

	protected Animator _animator;

	#endregion

	#region Properties

	public override TeamData Team
	{
		get
		{
			if (_team == null)
				_team = TurnBasedManager.Instance.Teams.First(x => !x.Playable);
			return _team;
		}
	}

	#endregion

	protected override void Awake()
	{
		base.Awake();
		_damageable = GetComponent<IDamageProcessor>();
		_navigation = GetComponent<Navigation>();
		_animator = GetComponentInChildren<Animator>();
	}

	protected override void OnAvailable()
	{
		if (!IsPlaying || _damageable.IsDead) return;

		if (_beginTurnHealth != Health.Value)
		{
			_beginTurnHealth = Health.Value;
			ProcessCostMap();
		}

		if (_actionQueue.Count == 0)
		{
			TurnBasedManager.Instance.EndTurn();
			return;
		}

		ActionNodePair action = _actionQueue.Dequeue();

		switch (action.Action)
		{
			case Action.MOVE:
				_navigation.MoveToNode(action.Node);
				break;
			case Action.ATTACK:
				if (action.Node.Entities.Contains(GameManager.Instance.King) && GameManager.Instance.King.Health.Value <= Damage.Value)
					GameManager.Instance.OnKingDeath();
				OnAttack(action.Node);
				break;
			default:
				TurnBasedManager.Instance.EndTurn();
				break;
		}
	}

	protected abstract void OnAttack(Node node);

	private int GetEntityValue(EntityType entity)
	{
		if (!(entity is EntityIdentity)) return 0;

		EntityData identity = ((EntityIdentity)entity).Identity;

		if (!_targetWorth.WorthTable.ContainsKey(identity))
			return 0;
		return _targetWorth.WorthTable[identity];
	}

	private void AddToCostMap(Node key, NodeValue value)
	{
		if (!_costMap.ContainsKey(key))
			_costMap[key] = new List<NodeValue>();
		_costMap[key].Add(value);
	}

	private void CalculateCosts()
	{
		// Getting the naive full range of the enemy (mp + range), not taking the obstacles into consideration
		List<Node> _naiveRange = EntityMap.Instance.GetFullRange(CurrentNode, MovementPoints.Value + Range.Value, false, new List<Node>());
		// Getting the range where the enemy can walk
		List<Node> _walkables = EntityMap.Instance.GetWalkableSurroundings(CurrentNode, MovementPoints.Value);
		// Getting all player entities in the enemy naive full range
		List<EntityType> _reacheableEntities = _naiveRange
									.Where(x => !x.IsEmpty)
									.SelectMany(x => x.Entities)
									.Where(x => GetEntityValue(x) > 0).ToList();

		for (int i = 0; i < _reacheableEntities.Count; i++)
		{
			EntityType entity = _reacheableEntities[i];
			var reachFromEntity = EntityMap.Instance.GetVisibleNodes(entity.CurrentNode, this.Range.Value);
			foreach (Node node in reachFromEntity)
			{
				if (_walkables.Contains(node))
					AddToCostMap(node, new NodeValue(entity.CurrentNode, GetEntityValue(entity)));
			}
		}
		foreach (Node node in _walkables)
			AddToCostMap(node, new NodeValue(null, -(int)node.DistanceTo(GameManager.Instance.King.CurrentNode)));
	}

	private int GetTotalValue(Node node) => _costMap[node].Sum(x => x.Value);

	private void ProcessCostMap()
	{
		_costMap.Clear();
		_actionQueue.Clear();

		CalculateCosts();

		if (_costMap.Count == 0)
			return;

		KeyValuePair<Node, List<NodeValue>> bestValue = _costMap.Aggregate((a, b) => GetTotalValue(a.Key) > GetTotalValue(b.Key) ? a : b);
		NodeValue bestNodeValue = bestValue.Value.Aggregate((a, b) => a.Value > b.Value ? a : b);

		if (CurrentNode != bestValue.Key)
			_actionQueue.Enqueue(new ActionNodePair(Action.MOVE, bestValue.Key));
		if (bestNodeValue.Source != null)
			_actionQueue.Enqueue(new ActionNodePair(Action.ATTACK, bestNodeValue.Source));
		else
		{
			NodeValue? hit = _costMap[bestValue.Key].FirstOrDefault(x => x.Source != null);

			if (hit == null || hit.Value.Source == null)
				return;
			_actionQueue.Enqueue(new ActionNodePair(Action.ATTACK, hit.Value.Source));
		}
	}

	public override void OnTurnBegin()
	{
		base.OnTurnBegin();

		_beginTurnHealth = Health.Value;
		ProcessCostMap();
		OnAvailable();
	}

	public override void OnTurnEnd()
	{
		base.OnTurnEnd();
		_actionQueue.Clear();
		_costMap.Clear();
	}
}