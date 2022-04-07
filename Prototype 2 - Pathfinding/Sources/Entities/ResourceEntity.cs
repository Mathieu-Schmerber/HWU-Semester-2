using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ResourceEntity : EntityType
{
	private ResourceData _data;
	[DisplayName("$NAME")] public string Name => _data?.Name;
	[DisplayName("$WOOD")] public int? Wood => _data.WoodGain?.Value;
	[DisplayName("$METAL")] public int? Metal => _data.MetalGain?.Value;

	public void Init(ResourceData data)
	{
		_data = data;
		Instantiate(_data.GFX, transform);
	}

	public override void OnEntityCross(ATurnBasedEntity entity)
	{
		if (!(entity is Soldier)) return;

		GameManager.Instance.MetalAmount += Metal ?? 0;
		GameManager.Instance.WoodAmount += Wood ?? 0;

		EntityMap.Instance.RemoveEntity(this);
		Tween.LocalScale(transform.GetChild(0), Vector3.zero, 0.3f, 0, Tween.EaseIn);
		Tween.LocalPosition(transform.GetChild(0), Vector3.up, 0.3f, 0, Tween.EaseIn);
		Destroy(gameObject, 0.3f);
	}
}
