using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseableMenu : MonoBehaviour
{
	[SerializeField] private Vector2 _movementRatio;
	[SerializeField] private float _movementDuration = 0.3f;
	public bool IsOpen = true;

	private Vector2 _closedPos;
	private Vector2 _openedPos;
	private RectTransform _rectTranform;
	private CanvasGroup _group;

	private Vector2 GetOffset()
	{
		return new Vector2(_movementRatio.x * _rectTranform.sizeDelta.x, _movementRatio.y * _rectTranform.sizeDelta.y);
	}

	private void Awake()
	{
		_group = GetComponent<CanvasGroup>();
		_rectTranform = GetComponent<RectTransform>();

		_openedPos = IsOpen ? _rectTranform.anchoredPosition : _rectTranform.anchoredPosition + GetOffset();
		_closedPos = !IsOpen ? _rectTranform.anchoredPosition : _rectTranform.anchoredPosition - GetOffset();
	}

	public void SetInteractivity(bool state) => _group.interactable = state;

	[Button("Open")]
	public void Open()
	{
		SetInteractivity(true);
		if (IsOpen) return;

		if (Application.isPlaying)
			Tween.AnchoredPosition(_rectTranform, _openedPos, _movementDuration, 0);
		else
		{
			_group = GetComponent<CanvasGroup>();
			_rectTranform = GetComponent<RectTransform>();
			_rectTranform.anchoredPosition += GetOffset();
		}
		IsOpen = true;
	}

	[Button("Close")]
	public void Close()
	{
		SetInteractivity(false);
		if (!IsOpen) return;

		if (Application.isPlaying)
			Tween.AnchoredPosition(_rectTranform, _closedPos, _movementDuration, 0);
		else
		{
			_group = GetComponent<CanvasGroup>();
			_rectTranform = GetComponent<RectTransform>();
			_rectTranform.anchoredPosition -= GetOffset();
		}
		IsOpen = false;
	}

	public void MoveMenu(Vector2 position)
	{
		_rectTranform.anchoredPosition = position;
	}
}
