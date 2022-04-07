using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodScreenEffect : MonoBehaviour
{
	private Damageable _player;
	private Image _image;

	private void Awake()
	{
		// TODO: replace with some manager reference
		_player = GameObject.FindObjectOfType<PlayerController>().GetComponent<Damageable>();
		_image = GetComponent<Image>();
	}

	private void OnEnable()
	{
		_player.OnDamage += PlayBloodEffect;
	}

	private void OnDisable()
	{
		_player.OnDamage -= PlayBloodEffect;
	}

	private void PlayBloodEffect()
	{
		float invisibleAlpha = 0f;
		float maxAlpha = 0.5f;

		Tween.Value(invisibleAlpha, maxAlpha, (float newValue) => _image.color = new Color(255, 255, 255, newValue), 0.1f, 0, Tween.EaseOut);
		Tween.Value(maxAlpha, invisibleAlpha, (float newValue) => _image.color = new Color(255, 255, 255, newValue), 0.1f, 0.1f, Tween.EaseIn);
	}
}
