using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Defines a playable turn based entity
/// </summary>
public abstract class PlayableEntity : ATurnBasedEntity
{
	#region Types

	public enum Action
	{
		MOVEMENT,
		ATTACK,
		BUILD
	}

	#endregion

	[SerializeField] private bool _canBuild = false;

	private TeamData _team;
	protected PreviewData _previewData;
	private Navigation _navigation;
	protected Action _state;

	#region Properties

	public bool CanBuild => _canBuild;
	public override TeamData Team {
		get
		{
			if (_team == null)
				_team = TurnBasedManager.Instance.Teams.First(x => x.Playable);
			return _team;
		}
	}

	#endregion

	#region Unity builtins

	protected override void Awake()
	{
		base.Awake();
		_previewData = ResourceLoader.GetPreviewData();
		_navigation = GetComponent<Navigation>();
	}

	protected virtual void OnEnable()
	{
		CellManager.Instance.OnNodeClicked += OnNodeClicked;
		CellManager.Instance.OnNodeSelected += OnNodeSelected;
		CellManager.Instance.OnPreviewCanceled += OnPreviewCanceled;
		ActionPanel.OnAttackButtonPressed += OnAttackBtnPressed;
	}

	protected virtual void OnDisable()
	{
		CellManager.Instance.OnNodeClicked -= OnNodeClicked;
		CellManager.Instance.OnNodeSelected -= OnNodeSelected;
		CellManager.Instance.OnPreviewCanceled -= OnPreviewCanceled;
		ActionPanel.OnAttackButtonPressed -= OnAttackBtnPressed; 
	}

	#endregion

	#region Previews

	/// <summary>
	/// Shows the appropriate cell preview depending on the Entity action state
	/// </summary>
	protected void ShowPreview()
	{
		if (!IsPlaying) return;
		List<Node> preview;

		// Stops the previous preview
		CellManager.Instance.StopPreview();
		switch (_state)
		{
			case Action.MOVEMENT:
				if (MovementPoints?.Value == 0 || _navigation == null) return;
				preview = EntityMap.Instance.GetWalkableSurroundings(CurrentNode, MovementPoints.Value);
				CellManager.Instance.PreviewCells(preview, _previewData.MovementPreviewColor, isPersistent: true);
				break;
			case Action.ATTACK:
				preview = EntityMap.Instance.GetVisibleNodes(CurrentNode, Range.Value);
				CellManager.Instance.PreviewCells(preview, _previewData.RangePreviewColor);
				break;
			case Action.BUILD:
				ShowBuildPreview();
				break;
		}
	}

	#endregion

	#region Actions

	protected virtual void OnPreviewCanceled()
	{
		_state = Action.MOVEMENT;
		ShowPreview();
	}

	private void OnNodeSelected(Node node)
	{
		if (!IsPlaying) return;

		switch (_state)
		{
			case Action.MOVEMENT:
				var path = EntityMap.Instance.GetPath(CurrentNode, node);
				CellManager.Instance.StopHightlight();
				CellManager.Instance.Highlight(path);
				break;
			case Action.ATTACK:
				break;
			case Action.BUILD:
				if (!node.IsEmpty) return;
				OnBuildNodeSelected(node);
				break;
		}
	}

	private void OnNodeClicked(Node node)
	{
		if (!IsPlaying || (_state != Action.MOVEMENT && HasExecutedAnAction)) return;
		else if (IsBusy) return;

		CellManager.Instance.StopPreview();
		switch (_state)
		{
			case Action.MOVEMENT:
				_navigation?.MoveToNode(node);
				break;
			case Action.ATTACK:
				if (node.Entities.Contains(GameManager.Instance.King) && GameManager.Instance.King.Health.Value <= Damage.Value)
					GameManager.Instance.OnKingDeath();
				Attack(node);
				HasExecutedAnAction = true;
				break;
			case Action.BUILD:
				if (!node.IsEmpty) return;
				BuildOnNode(node);
				break;
			default:
				break;
		}
	}

	protected abstract void Attack(Node node);

	#endregion

	protected virtual void OnBuildNodeSelected(Node node) {}

	protected virtual void BuildOnNode(Node node) {}

	protected virtual void ShowBuildPreview() {}

	private void OnAttackBtnPressed()
	{
		if (!IsPlaying || HasExecutedAnAction) return;

		_state = Action.ATTACK;
		ShowPreview();
	}

	protected override void OnAvailable()
	{
		if (!IsPlaying) return;

		_state = Action.MOVEMENT;
		ShowPreview();
	}

	public override void OnTurnBegin()
	{
		base.OnTurnBegin();
		OnAvailable();
	}

	public override void OnTurnEnd()
	{
		base.OnTurnEnd();
		CellManager.Instance.StopPreview();
	}
}
