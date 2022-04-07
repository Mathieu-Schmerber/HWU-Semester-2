using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : BindableTextData
{
	private Camera _camera;
	private RectTransform _rectTransform;
	private Canvas _canvas;

	protected override void Awake()
	{
		base.Awake();
		_canvas = GetComponentInParent<Canvas>();
		_camera = Camera.main;
		_rectTransform = GetComponent<RectTransform>();
	}

	/// <summary>
	/// Sets the tooltip position over the object on the 2D screen space
	/// </summary>
	/// <param name="following"></param>
	public void SetPosition(GameObject following, Vector3 padding)
	{
		if (following == null)
			return;

		RectTransform ui = following.GetComponent<RectTransform>();

		if (ui != null)
		{
			var tooltipRect = RectTransformUtility.PixelAdjustRect(_rectTransform, _canvas);
			var uiRect = RectTransformUtility.PixelAdjustRect(ui, _canvas);

			transform.position = ui.position + new Vector3(0, (uiRect.height + tooltipRect.height) * _canvas.scaleFactor, 0);
			return;
		}

		Vector3 gameObjectOffset = following.GetComponent<BoxCollider>().size.y * Vector3.up;
		Vector3 screenSpace = _camera.WorldToScreenPoint(following.transform.position + gameObjectOffset + padding);

		transform.position = screenSpace;
	}

	public override void OnValueNull(Text[] textsObject, string code) {}

	public override void OnValueChanging(Text[] textsObject, string code, object value) {}

	public override void OnValueChanged(Text[] textsObject, string code, object value) {}
}