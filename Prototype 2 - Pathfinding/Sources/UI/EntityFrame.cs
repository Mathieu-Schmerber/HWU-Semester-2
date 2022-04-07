using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines an entity icon logic in the TurnBasedUI
/// </summary>
public class EntityFrame : MonoBehaviour, ISelectableListener
{
	#region Fields

	[SerializeField] private Image _iconUi;
	[SerializeField] private Slider _highlightFrameUi;
	[SerializeField] private Slider _healthSlider;

	private Image _highlightBackground;
	private Image _highlightFill;
	private Vector3 _baseScale;

	#endregion

	#region Properties

	private List<ISelectableListener> _entitySelectables;
	public EntityIdentity Entity { get; set; }

	#endregion

	#region Unity builtins

	private void Awake()
	{
		ATurnBasedEntity.OnTurnBegan += ATurnBasedEntity_OnTurnBegan;

		Image[] images = _highlightFrameUi.GetComponentsInChildren<Image>();
		_highlightBackground = images[0];
		_highlightFill = images[1];
		_highlightFrameUi.maxValue = TurnBasedManager.Instance.MaxTurnTime;
		_baseScale = gameObject.transform.localScale;
	}

	private void OnDestroy()
	{
		ATurnBasedEntity.OnTurnBegan -= ATurnBasedEntity_OnTurnBegan;
	}

	private void Update()
	{
		if (Entity != null)
		{
			_healthSlider.value = Entity.Health.Value;
			_highlightFrameUi.value = TurnBasedManager.Instance.ElapsedTime;
		}
	}

	#endregion

	/// <summary>
	/// Shows which entity is currently playing
	/// </summary>
	/// <param name="obj"></param>
	private void ATurnBasedEntity_OnTurnBegan(ATurnBasedEntity obj)
	{
		if (obj.gameObject == Entity.gameObject)
		{
			iTween.ScaleTo(gameObject, _baseScale + (Vector3.one * 0.2f), 0.3f);
			_highlightFrameUi.gameObject.SetActive(true);
			_highlightFill.color = obj.Team.PrimaryColor;
			_highlightBackground.color = obj.Team.SecondaryColor;
		}
		else
		{
			iTween.ScaleTo(gameObject, _baseScale, 0.3f);
			_highlightFrameUi.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Sets up the UI based on the assigned entity
	/// </summary>
	/// <param name="entity"></param>
	public void SetEntity(EntityIdentity entity)
	{
		Entity = entity;
		_iconUi.sprite = Entity.Icon;
		if (entity.Health != null)
			_healthSlider.maxValue = entity.Health.Max;
		_entitySelectables = Entity.GetComponents<ISelectableListener>().ToList();
	}

	/// <summary>
	/// Selects the entity when selecting the UI
	/// </summary>
	public void OnSelect()
	{
		_entitySelectables.ForEach(x => x.OnSelect());
	}

	/// <summary>
	/// DeSelects the entity when deselecting the UI
	/// </summary>
	public void OnDeselect()
	{
		_entitySelectables.ForEach(x => x.OnDeselect());
	}

	public void OnKeepSelecting() { }
}