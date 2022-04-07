using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIndicatorUI : MonoBehaviour
{
    [SerializeField] private Sprite _active;
    [SerializeField] private Sprite _inactive;

	private Color _baseColor;
	private ShopItemUI _ui;
	private Image _img;

	private void Awake()
	{
		_ui = GetComponentInParent<ShopItemUI>();
		_img = GetComponent<Image>();
		_baseColor = _img.color;
	}

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		int index = transform.GetSiblingIndex();
		Sprite before = _img.sprite;

		_img.sprite = (index <= _ui.Data.Equipment.CurrentUpgrade && _ui.Data.Equipment.IsEquipped) ? _active : _inactive;
		if (before != _img.sprite)
			Tween.Value(Color.white, _baseColor, (color) => _img.color = color, 0.3f, 0, Tween.EaseOut);
	}
}
