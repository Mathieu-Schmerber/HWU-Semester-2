using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanelManager : MonoBehaviour
{
    [SerializeField] private CloseableMenu _actionMenu;
    [SerializeField] private CloseableMenu _buildMenu;

	private void OnEnable()
	{
		ATurnBasedEntity.OnTurnBegan += ATurnBasedEntity_OnTurnBegan;
		TurnBasedManager.Instance.OnWillEndTurn += Instance_OnWillEndTurn;
	}

	private void OnDisable()
	{
		ATurnBasedEntity.OnTurnBegan -= ATurnBasedEntity_OnTurnBegan;
		if (TurnBasedManager.Instance != null)
			TurnBasedManager.Instance.OnWillEndTurn -= Instance_OnWillEndTurn;
	}

	private void Instance_OnWillEndTurn()
	{
		if (_actionMenu.IsOpen)
			_actionMenu.Close();
		if (_buildMenu.IsOpen)
			_buildMenu.Close();
	}

	private void ATurnBasedEntity_OnTurnBegan(ATurnBasedEntity playingEntity)
	{
		PlayableEntity playable = playingEntity as PlayableEntity;

		if (!playable)
			_actionMenu.Close();
		else
		{
			_actionMenu.Open();
			if (playable.CanBuild)
				_buildMenu.Open();
			else
				_buildMenu.Close();
		}
	}
}
