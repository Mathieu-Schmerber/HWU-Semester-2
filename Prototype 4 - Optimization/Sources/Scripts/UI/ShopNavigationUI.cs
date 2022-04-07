using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Saving;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNavigationUI : MonoBehaviour, INavigationUI
{
	private ShopListUI[] _pages;
	[SerializeField] private Text _pageTitle;
	[SerializeField] private Text _coinsAmountTxt;
	[SerializeField] private Text _buyText;
	[SerializeField] private Button _previousPageBtn;
	[SerializeField] private Button _nextPageBtn;
	[SerializeField] private Button _buyBtn;
	[SerializeField] private Button _backButton;

	private int _activePageIndex = 0;
	private ShopItemData _selected;
	private CanvasGroup _canvasGroup;
	private RectTransform _rect;
	private MenuManagement _menuManagement;

	public static event Action<ShopItemData, int> OnItemBought;

	public ShopItemData Selected { get => _selected; 
		set {
			_selected = value;
			OnItemSelected(value);
		} 
	}

	#region Unity builtins

	private void OnEnable()
	{
		GameStats.Instance.OnCoinsUpdated += OnCoinsAmountChanged;
	}

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_rect = GetComponent<RectTransform>();
		_menuManagement = GetComponentInParent<MenuManagement>();
		_pages = GetComponentsInChildren<ShopListUI>();
		_previousPageBtn.onClick.AddListener(PreviousPageBtn_Pressed);
		_nextPageBtn.onClick.AddListener(NextPageBtn_Pressed);
		_buyBtn.onClick.AddListener(BuySelectionBtn_Pressed);
		_backButton.onClick.AddListener(_menuManagement.ShowMainMenu);
	}

	private void Start()
	{
		OnCoinsAmountChanged(GameStats.Instance.Coins);
		_pages.ForEach(x => x.gameObject.SetActive(false));
		_pages[_activePageIndex].gameObject.SetActive(true);
		_pageTitle.text = _pages[_activePageIndex].ListTitle;
	}

	#endregion

	private void OnItemSelected(ShopItemData item)
	{
		if (item == null || !item.Equipment.HasNextUpgrade)
			_buyBtn.interactable = false;
		else
			_buyBtn.interactable = true;
		Tween.LocalScale(_buyBtn.transform, Vector3.one * (_buyBtn.interactable ? 1.1f : 1), 0.1f, 0.1f);
	}

	private void NextPageBtn_Pressed()
	{
		// Error check
		if (_activePageIndex + 1 >= _pages.Length)
			return;

		// Page switch
		_pages[_activePageIndex].Hide(true);
		_pages[_activePageIndex].SelectItem(null);
		_activePageIndex++;
		_pages[_activePageIndex].Show(true);
		_pages[_activePageIndex].SelectItem(null);
		_pageTitle.text = _pages[_activePageIndex].ListTitle;
		// Interactibility
		_previousPageBtn.interactable = true;
		if (_activePageIndex + 1 >= _pages.Length)
			_nextPageBtn.interactable = false;
		Tween.LocalScale(_nextPageBtn.transform, Vector3.one * 1.4f, 0.1f, 0);
		Tween.LocalScale(_nextPageBtn.transform, Vector3.one, 0.1f, 0.1f);
	}

	private void PreviousPageBtn_Pressed()
	{
		// Error check
		if (_activePageIndex - 1 < 0)
			return;

		// Page switch
		_pages[_activePageIndex].Hide(false);
		_pages[_activePageIndex].SelectItem(null);
		_activePageIndex--;
		_pages[_activePageIndex].Show(false);
		_pages[_activePageIndex].SelectItem(null);
		_pageTitle.text = _pages[_activePageIndex].ListTitle;

		// Interactibility
		_nextPageBtn.interactable = true;
		if (_activePageIndex == 0)
			_previousPageBtn.interactable = false;
		Tween.LocalScale(_previousPageBtn.transform, Vector3.one * 1.4f, 0.1f, 0);
		Tween.LocalScale(_previousPageBtn.transform, Vector3.one, 0.1f, 0.1f);
	}

	private void OnCoinsAmountChanged(int amount)
	{
		_coinsAmountTxt.text = $"{amount}¤";
		Tween.LocalScale(_coinsAmountTxt.transform, Vector3.one * 1.4f, 0.1f, 0);
		Tween.LocalScale(_coinsAmountTxt.transform, Vector3.one, 0.1f, 0.1f);
	}

	private void BuySelectionBtn_Pressed()
	{
		if (_selected != null && GameStats.Instance.Coins >= _selected.Price)
		{
			OnItemBought?.Invoke(_selected, _selected.Price);
			SaveSystem.Save();
			OnItemSelected(_selected);
			_pages[_activePageIndex].RefreshList();
		} else
		{
			_buyText.text = "Not enough coins at hand.";
			_buyText.color = Color.red;
			Tween.Stop(_buyText.transform.GetInstanceID());
			Tween.LocalScale(_buyText.transform, Vector3.one * 1.1f, 0.1f, 0);
			Tween.Value(new Color(0, 0, 0, 0), Color.red, (color) => _buyText.color = color, .1f, 0);
			Tween.LocalScale(_buyText.transform, Vector3.one, 0.1f, 0.1f);
			Tween.Value(_buyText.color, new Color(0, 0, 0, 0), (color) => _buyText.color = color, .5f, 1f);
		}
	}

	public void Close()
	{
		if (!gameObject.activeSelf) return;

		Vector3 destination = -Vector3.up * Screen.height;

		_canvasGroup.interactable = false;
		Tween.CanvasGroupAlpha(_canvasGroup, 0, 0.3f, 0, Tween.EaseIn, completeCallback: () => gameObject.SetActive(false));
		Tween.AnchoredPosition(_rect, Vector3.zero, destination, 0.3f, 0, Tween.EaseIn);
	}

	public void Open()
	{
		gameObject.SetActive(true);
		_canvasGroup.interactable = true;
		Tween.CanvasGroupAlpha(_canvasGroup, 1, 0.3f, 0, Tween.EaseOut);
		Tween.AnchoredPosition(_rect, Vector3.zero, 0.3f, 0, Tween.EaseOut);

		_pages.ForEach(x => x.RefreshList());
	}
}