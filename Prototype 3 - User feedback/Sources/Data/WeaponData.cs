using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponData : SerializedScriptableObject
{
	#region Types

	[System.Serializable]
	public class Attack
	{
		[AssetsOnly, AssetSelector(Paths = "Assets/Resources/Prefabs/Attacks"), 
		ValidateInput(nameof(AssertIsAttack), "Prefab should have a PlayerAttack component")]
		public GameObject Prefab;
		public Vector3 StartOffset;
		public Vector3 TravelDistance;
		public bool AimAssist;
		private bool AssertIsAttack(GameObject prefab) => prefab != null && prefab.GetComponent<PlayerAttack>() != null;
	}

	[System.Serializable]
	public class Dash
	{
		public bool OnlyWhenMoving;
		public bool OnAnimationEventOnly;
		public float Distance;
		public float Duration;
		public int AfterImages;
	}

	[System.Serializable]
	public class WeaponAttack
	{
		[Required] public AnimationClip AttackAnimation;

		[TabGroup("Slash"), HideLabel] public Attack Attack;
		[TabGroup("Dash"), HideLabel] public Dash Dash;
		public int OnHitDamage;
	}

	#endregion

	#region Properties

	public float AttackSpeed;
	public string LocomotionLayer = "DefaultLocomotion";
	public GameObject Prefab;
	public float ComboIntervalTime;
	public List<WeaponAttack> AttackCombos;

	#endregion
}
