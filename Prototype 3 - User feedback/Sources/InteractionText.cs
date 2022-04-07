using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionText : MonoBehaviour
{
	private PlayerWeapon _player;
	private Text _text;

	private void Awake()
	{
		// TODO: replace with some manager reference
		_player = GameObject.FindObjectOfType<PlayerWeapon>();
		_text = GetComponent<Text>();
	}

	private void OnEnable()
	{
		_player.OnSuggestionChangedEvt += _player_OnSuggestionChangedEvt;
	}

	private void OnDisable()
	{
		_player.OnSuggestionChangedEvt -= _player_OnSuggestionChangedEvt;
	}

	private void _player_OnSuggestionChangedEvt(WeaponItem obj)
	{
		if (obj == null)
			_text.text = "";
		else
			_text.text = $"[Y] - Pickup {obj.name}";
	}
}
