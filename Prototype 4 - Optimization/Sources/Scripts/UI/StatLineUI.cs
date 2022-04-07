using Nawlian.Lib.Extensions;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatLineUI : MonoBehaviour
{
    private RectTransform _rect;
    private Text[] _texts;
	private Vector3 _basePos;
	private readonly Color _transparent = new Color(0, 0, 0, 0);

	private void Awake()
	{
		_rect = GetComponent<RectTransform>();
		_texts = GetComponentsInChildren<Text>();
		_basePos = _rect.anchoredPosition;
	}

	public void Show(float delay)
	{
		Tween.AnchoredPosition(_rect, Vector3.zero, _basePos, 0.5f, delay);
		Tween.Value(_transparent, Color.white, (color) => _texts.ForEach(x => x.color = color), 0.5f, delay);
	}

	public void SetValue(string value) => _texts[1].text = value;
}
