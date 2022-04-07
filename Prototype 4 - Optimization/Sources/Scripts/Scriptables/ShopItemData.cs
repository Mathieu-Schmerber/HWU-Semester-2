using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ShopItem")]
public class ShopItemData : ScriptableObject
{
	public Sprite Icon;
	public string Description;
	public int BasePrice;
	public AnimationCurve PriceInflation;
	[InlineEditor] public Equipment Equipment;

	public int Upgrades => Equipment?.UpgradeCount ?? 0;
	public int Price { 
		get
		{
			float ratio = 1f / Upgrades;
			int step = Equipment.IsEquipped ? Equipment.CurrentUpgrade + 1 : Equipment.CurrentUpgrade;

			return BasePrice + (int)(PriceInflation.Evaluate(ratio * step) * BasePrice);
		} 
	}
}
