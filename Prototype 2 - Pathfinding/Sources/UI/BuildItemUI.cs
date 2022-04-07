using Pixelplacement;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildItemUI : MonoBehaviour, ISelectableListener
{
	public struct BuildData
	{
		[DisplayName("$NAME")] public string Name { get; set; }
		[DisplayName("$HP")] public int? Health { get; set; }
		[DisplayName("$MP")] public int? MovementPoints { get; set; }
		[DisplayName("$RNG")] public int? Range { get; set; }
		[DisplayName("$DMG")] public int? Attack { get; set; }
		[DisplayName("$WCOST")] public int? WoodCost { get; set; }
		[DisplayName("$MCOST")] public int? MetalCost { get; set; }
	}

	[SerializeField] private Image _icon;
	[SerializeField] private Vector3 _animationIntensity;
	[SerializeField] private float _animationTime;

	private BuildData _tooltipData;
	private Button _btn;
	private EntityData _data;

	public static event Action<EntityData> OnBuildSelected;

	private void Awake()
	{
		_btn = GetComponent<Button>();
	}

	private void OnEnable()
	{
		ATurnBasedEntity.OnTurnActionExecuting += ATurnBasedEntity_OnTurnActionExecuting;
	}

	private void OnDisable()
	{
		ATurnBasedEntity.OnTurnActionExecuting -= ATurnBasedEntity_OnTurnActionExecuting;
	}

	private void ATurnBasedEntity_OnTurnActionExecuting(ATurnBasedEntity obj)
	{
		if (obj.IsPlaying)
			_btn.interactable = false;
	}
	public void Init(EntityData data) 
	{
		_data = data;
		_icon.sprite = _data.Icon;
		_tooltipData = new BuildData()
		{
			Name = data.Name,
			Health = data.Health.Max,
			MovementPoints = data.MovementPoints.Max,
			Attack = data.Damage.Max,
			Range = data.Range.Max,
			WoodCost = data.IsBuild.WoodCost,
			MetalCost = data.IsBuild.MetalCost
		};

		GetComponent<Button>().onClick.AddListener(OnPressed);
		_btn.interactable = GameManager.Instance.WoodAmount >= _data.IsBuild.WoodCost && GameManager.Instance.MetalAmount >= _data.IsBuild.MetalCost;
	}

	public void OnPressed()
	{
		if (!TurnBasedManager.Instance.CurrentlyPlayingEntity.HasExecutedAnAction)
		{
			iTween.PunchScale(gameObject, _animationIntensity, _animationTime);
			OnBuildSelected?.Invoke(_data);
		}
	}

	private void Update()
	{
		if (!TurnBasedManager.Instance.Started) return;

		bool state = _btn.interactable;

		_btn.interactable = !TurnBasedManager.Instance.CurrentlyPlayingEntity.HasExecutedAnAction 
			&& GameManager.Instance.WoodAmount >= _data.IsBuild.WoodCost && GameManager.Instance.MetalAmount >= _data.IsBuild.MetalCost;
		if (_btn.interactable != state)
			iTween.PunchScale(gameObject, _animationIntensity, _animationTime);
	}

	public void OnSelect()
		=> TooltipManager.Instance.Show(TooltipManager.TooltipType.STATS, gameObject, _tooltipData);

	public void OnKeepSelecting() {}

	public void OnDeselect()
		=> TooltipManager.Instance.Hide(TooltipManager.TooltipType.STATS);

}