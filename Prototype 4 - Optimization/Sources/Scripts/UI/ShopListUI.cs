using Nawlian.Lib.Extensions;
using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ShopListUI : MonoBehaviour
{
	[SerializeField, FolderPath(ParentFolder = "Assets/Resources")] private string _itemsFolder;
	[SerializeField] private ShopItemUI _shopItemPrefab;
	[SerializeField] private string _listTitle;

	private ShopItemUI[] shopItemUIs;
	private ShopItemData[] _shopItems;
	private CanvasGroup _group;
	private RectTransform _rect;
	private ShopNavigationUI _navigation;

	public string ListTitle => _listTitle;

	private void Awake()
	{
		_navigation = GetComponentInParent<ShopNavigationUI>();
		_shopItems = Resources.LoadAll<ShopItemData>(_itemsFolder);
		_group = GetComponent<CanvasGroup>();
		_rect = GetComponent<RectTransform>();

		foreach (ShopItemData item in _shopItems)
		{
			ShopItemUI ui = Instantiate(_shopItemPrefab, transform).GetComponent<ShopItemUI>();
			ui.SetupUI(item, SelectItem);
		}
		shopItemUIs = GetComponentsInChildren<ShopItemUI>();
	}

	public void SelectItem(ShopItemUI item)
	{
		shopItemUIs.ForEach(x => x.Deselect());
		item?.Select();
		_navigation.Selected = item?.Data;
	}

	public void RefreshList() => shopItemUIs.ForEach(x => x.Refresh());

	public void Show(bool isNext)
	{
		int sideFactor = isNext ? 1 : -1;
		Vector3 side = transform.right * Screen.width * sideFactor;

		_group.interactable = true;
		Tween.CanvasGroupAlpha(_group, 0, 1, 0.5f, 0, startCallback: () => gameObject.SetActive(true));
		Tween.AnchoredPosition(_rect, side, Vector3.zero, 0.5f, 0, Tween.EaseInOut);
	}

	public void Hide(bool isNext)
	{
		int sideFactor = isNext ? -1 : 1;
		Vector3 side = transform.right * Screen.width * sideFactor;

		_group.interactable = false;
		Tween.CanvasGroupAlpha(_group, 1, 0, 0.5f, 0, completeCallback: () => gameObject.SetActive(false));
		Tween.AnchoredPosition(_rect, Vector3.zero, side, 0.5f, 0, Tween.EaseInOut);
	}
}
