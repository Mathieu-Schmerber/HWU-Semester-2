using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _arrow;
    [SerializeField] private SpriteRenderer _floor;

	private Animator _animator;
    private ATurnBasedEntity _entity;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_entity = GetComponentInParent<ATurnBasedEntity>();
		_arrow.color = _entity.Team.PrimaryColor;
		_floor.color = _entity.Team.SecondaryColor;

		ATurnBasedEntity.OnTurnBegan += ATurnBasedEntity_OnTurnBegan;
		ATurnBasedEntity.OnTurnEnded += ATurnBasedEntity_OnTurnEnded;
	}

	private void OnDestroy()
	{
		ATurnBasedEntity.OnTurnBegan -= ATurnBasedEntity_OnTurnBegan;
		ATurnBasedEntity.OnTurnEnded -= ATurnBasedEntity_OnTurnEnded;
	}

	private void ATurnBasedEntity_OnTurnEnded(ATurnBasedEntity obj)
	{
		_animator.SetBool("IsPlaying", false);
	}

	private void ATurnBasedEntity_OnTurnBegan(ATurnBasedEntity obj)
	{
		_animator.SetBool("IsPlaying", (obj == _entity));
	}
}
