using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Defines the ability to select UI with the mouse.
/// </summary>
public class UiMouseSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private List<ISelectableListener> _selectable;

	private void Awake()
	{
		_selectable = GetComponents<ISelectableListener>().ToList();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (enabled)
			_selectable.ForEach(x => x.OnSelect());
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (enabled)
			_selectable.ForEach(x => x.OnDeselect());
	}
}
