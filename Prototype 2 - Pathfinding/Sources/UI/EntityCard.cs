using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EntityCard : BindableTextData
{
	private Image _iconImage;
	private UnityEngine.UI.Outline _iconMask;

	#region Unity builtins

	protected override void Awake()
	{
		base.Awake();
		_iconMask = GetComponentInChildren<Mask>().GetComponent<UnityEngine.UI.Outline>();
		_iconImage = _iconMask.transform.GetChild(0).GetComponent<Image>();
	}

	private void OnEnable()
	{
		ATurnBasedEntity.OnTurnBegan += SetEntity;
	}
	private void OnDestroy()
	{
		ATurnBasedEntity.OnTurnBegan -= SetEntity;
	}

	#endregion

	public void SetEntity(ATurnBasedEntity entity)
	{
		_iconImage.sprite = entity.Icon;
		_iconMask.effectColor = new Color(entity.Team.PrimaryColor.r, entity.Team.PrimaryColor.g, entity.Team.PrimaryColor.b, _iconMask.effectColor.a);
		Bind(entity);
	}

	public override void OnValueNull(Text[] textsObject, string code)
	{
		textsObject.ForEach(x => x.text = "0");
	}
}
