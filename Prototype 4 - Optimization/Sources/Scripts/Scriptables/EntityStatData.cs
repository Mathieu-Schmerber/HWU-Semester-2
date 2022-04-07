using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Stats")]
public class EntityStatData : ScriptableObject
{
	public bool IsPlayerTeam;
	public int CoinWorth;

	public int BaseMaxHealth;
	public float BaseRegeneration;
	public float BaseSpeed;
	public float BaseManiability;
	public float BaseDamageMultiplier;
}