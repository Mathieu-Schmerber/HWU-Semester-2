using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [SerializeField] private Button _attackbtn;
    [SerializeField] private Vector3 _animationIntensity;
    [SerializeField] private float _animationTime;

    public static event Action OnAttackButtonPressed;
    public static event Action OnEndTurnButtonPressed;

	private void OnEnable()
	{
		ATurnBasedEntity.OnTurnBegan += ATurnBasedEntity_OnTurnBegan;
		ATurnBasedEntity.OnTurnActionExecuting += ATurnBasedEntity_OnTurnActionExecuting;
	}

	private void OnDisable()
	{
        ATurnBasedEntity.OnTurnBegan -= ATurnBasedEntity_OnTurnBegan;
        ATurnBasedEntity.OnTurnActionExecuting -= ATurnBasedEntity_OnTurnActionExecuting;
    }

	private void ATurnBasedEntity_OnTurnBegan(ATurnBasedEntity entity)
	{
        if (entity.IsPlaying)
            _attackbtn.interactable = true;
    }

	private void ATurnBasedEntity_OnTurnActionExecuting(ATurnBasedEntity entity)
	{
        if (entity.IsPlaying)
            _attackbtn.interactable = false;
	}

    public void AttackBtn()
    {
        if (!TurnBasedManager.Instance.Started) return;
        if (!TurnBasedManager.Instance.CurrentlyPlayingEntity.HasExecutedAnAction)
        {
            iTween.PunchScale(_attackbtn.gameObject, _animationIntensity, _animationTime);
            OnAttackButtonPressed?.Invoke();
        }
    }

    public void EndTurnBtn()
    {
        if (!TurnBasedManager.Instance.Started) return;

        OnEndTurnButtonPressed?.Invoke();
    }
}