using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour, ISelectableListener
{
	private Renderer _renderer;
	private Color _baseColor;

	public Node Node { get; set; }
	public bool IsHighlighted => _renderer.material.color != _baseColor;

	private void Awake()
	{
		_renderer = GetComponentInChildren<Renderer>();
	}

	private void OnEnable()
	{
		_baseColor = _renderer.material.color;
		_renderer.transform.localScale = Vector3.zero;

		// Animating the apparition
		Tween.LocalScale(_renderer.transform, Vector3.one, 0.3f, 0, Tween.EaseOut);
	}

	public void SetHighlight(bool state) => _renderer.material.color = _baseColor * (state ? 1.5f : 1f);

	public void OnSelect() => SetHighlight(true);

	public void OnDeselect() => CellManager.Instance.StopHightlight();

	public void OnKeepSelecting() { }
}
