using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
	[SerializeField] private GameObject _gameSelectionPanel;

	public static event Action<Gamemode.Mode> OnUserStartsGameEvt;

	private void Start()
	{
		Spawner.OnGameEndsEvt += Spawner_OnGameEndsEvt;
	}

	private void OnDestroy()
	{
		Spawner.OnGameEndsEvt -= Spawner_OnGameEndsEvt;
	}

	private void Spawner_OnGameEndsEvt(GameEndEventArgs gameStats)
	{
		// Todo: display the game stats in an other panel
		Cursor.visible = true;
		_gameSelectionPanel.SetActive(true);
	}

	private void PlayAs(Gamemode.Mode mode)
	{
		Cursor.visible = false;
		_gameSelectionPanel.SetActive(false);
		OnUserStartsGameEvt?.Invoke(mode);
	}

	public void HunterBtn() => PlayAs(Gamemode.Mode.HUNTER);
	public void WolfBtn() => PlayAs(Gamemode.Mode.WOLF);
	public void AiBtn() => PlayAs(Gamemode.Mode.AI_ONLY);
}
