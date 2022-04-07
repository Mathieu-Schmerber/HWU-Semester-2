using UnityEngine;

[CreateAssetMenu(menuName = "Data/Constants")]
public class ConstantData : ScriptableObject
{
	public LayerMask EnemyLayer;
	public Color PlayerColor;
	public Color EnemyColor;
}