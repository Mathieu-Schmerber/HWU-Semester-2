using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnDisplay : MonoBehaviour
{
	private Text _text;

	private void Awake()
	{
		_text = GetComponent<Text>();
	}

	private void OnEnable()
	{
		TurnBasedManager.Instance.OnFullTurnBegin += Instance_OnFullTurnBegin;
	}

	private void OnDisable()
	{
		if (TurnBasedManager.Instance != null)
			TurnBasedManager.Instance.OnFullTurnBegin -= Instance_OnFullTurnBegin;
	}

	private void Instance_OnFullTurnBegin(int number)
	{
		transform.localScale = Vector3.zero;
		_text.enabled = true;
		_text.text = $"Turn {number}";
		Tween.LocalScale(transform, Vector3.one, 0.2f, 0, completeCallback: () => iTween.PunchScale(gameObject, Vector3.one, 0.5f));
		Tween.LocalScale(transform, Vector3.zero, 0.2f, 1.5f, completeCallback: () => _text.enabled = false);
	}
}
