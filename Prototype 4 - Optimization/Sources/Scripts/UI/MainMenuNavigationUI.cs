using Nawlian.Lib.Systems.Saving;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuNavigationUI : MonoBehaviour, INavigationUI
{
	[SerializeField] private TriggerButtonUI _startBtn;
	[SerializeField] private TriggerButtonUI _upgradesBtn;
	[SerializeField] private TriggerButtonUI _statsBtn;

	private CanvasGroup _canvasGroup;
	private RectTransform _rect;
	private MenuManagement _menuManagement;

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_rect = GetComponent<RectTransform>();
		_menuManagement = GetComponentInParent<MenuManagement>();
		_canvasGroup.interactable = true;
	}

	private void Start()
	{
		_upgradesBtn.OnClick(_menuManagement.ShowShop);
		_startBtn.OnClick(_menuManagement.ShowInGame);
		_statsBtn.OnClick(_menuManagement.ShowStats);
	}

	public void Close()
	{
		if (!gameObject.activeSelf) return;

		Vector3 destination = Vector3.up * Screen.height;

		_canvasGroup.interactable = false;
		Tween.CanvasGroupAlpha(_canvasGroup, 0, 0.3f, 0, Tween.EaseIn, completeCallback: () => gameObject.SetActive(false));
		Tween.AnchoredPosition(_rect, Vector3.zero, destination, 0.3f, 0, Tween.EaseIn);
	}

	public void Open()
	{
		Vector3 closePos = Vector3.up * Screen.height;
		Vector3 destination = Vector3.zero;

		gameObject.SetActive(true);
		_canvasGroup.interactable = true;
		Tween.CanvasGroupAlpha(_canvasGroup, 1, 0.3f, 0, Tween.EaseOut);
		Tween.AnchoredPosition(_rect, closePos, destination, 0.3f, 0, Tween.EaseOut);
	}
}
