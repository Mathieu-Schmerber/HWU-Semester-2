using Nawlian.Lib.Extensions;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
	[SerializeField] private Text _name;
	[SerializeField] private Image _icon;
	[SerializeField] private LayoutGroup _upgradeList;
	[SerializeField] private Text _price;
	[SerializeField] private GameObject _upgradeIndicatorPrefab;

	private UpgradeIndicatorUI[] _indicators;
	private Button _btn;
	private ShopItemData _data;
	private bool _isSelected;

	public ShopItemData Data => _data;
	public bool Selected => _isSelected;

	private void Awake()
	{
		_btn = GetComponent<Button>();
	}

	public void SetupUI(ShopItemData data, Action<ShopItemUI> onSelect)
	{
		_data = data;
		_name.text = data.name;
		_icon.sprite = data.Icon;
		for (int i = 0; i < _data.Upgrades; i++)
			Instantiate(_upgradeIndicatorPrefab, _upgradeList.transform);
		_indicators = GetComponentsInChildren<UpgradeIndicatorUI>();
		_btn.onClick.AddListener(() => onSelect.Invoke(this));

		Deselect();
		Refresh();
	}

	public void Refresh()
	{
		if (_data.Equipment.HasNextUpgrade)
			_price.text = $"{_data.Price}¤";
		else
		{
			_price.color = Color.yellow;
			_price.text = "Sold out";
		}
		_indicators.ForEach(x => x.Refresh());
	}

	public void Select()
	{
		_btn.image.color = Color.white;
		Tween.LocalScale(transform, Vector3.one * 1.1f, 0.1f, 0);
	}

	public void Deselect()
	{
		_btn.image.color = new Color(0, 0, 0, 0);
		Tween.LocalScale(transform, Vector3.one, 0.1f, 0);
	}
}
