using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityIdentity;

[CreateAssetMenu(menuName = "Data/Entity")]
public class EntityData : ScriptableObject
{
	[System.Serializable]
	public class Cost : Toggleable
	{
		public int WoodCost;
		public int MetalCost;
	}

	public string Name;
	public Sprite Icon;

	public GameObject Prefab;

	public Cost IsBuild;
	public ToggleableValue<ResourceData> DropsResource;

	[FoldoutGroup("Stats")] public StatLine Health;
	[FoldoutGroup("Stats")] public StatLine MovementPoints;
	[FoldoutGroup("Stats")] public StatLine Range;
	[FoldoutGroup("Stats")] public StatLine Damage;
}
