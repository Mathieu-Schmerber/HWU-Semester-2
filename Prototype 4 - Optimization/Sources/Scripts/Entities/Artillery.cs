using Nawlian.Lib.Systems.Saving;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Artillery : MonoBehaviour, ISaveable
{
	#region Types

	[System.Serializable]
	public struct SaveData
	{
		public KeyValuePair<string, int>[] EquipmentIdUpgradePairs;
	}

	#endregion

	[SerializeField] private List<Equipment> _weapons;

	private EntityIdentity _identity;
	private IDamageProcessor _processor;

	private void Awake()
	{
		_processor = GetComponent<IDamageProcessor>();
		_identity = GetComponent<EntityIdentity>();
	}

	private void OnEnable()
	{
		ShopNavigationUI.OnItemBought += OnItemBought;
		GameLoop.OnGameStarted += GameLoop_OnGameStarted;
	}

	private void OnDisable()
	{
		ShopNavigationUI.OnItemBought -= OnItemBought;
		GameLoop.OnGameStarted -= GameLoop_OnGameStarted;
	}

	private void GameLoop_OnGameStarted()
	{
		_weapons.ForEach(x => x.OnEquip(_identity));
	}

	private void Update()
	{
		if (!GameLoop.HasGameStarted || _processor.IsDead) return;

		_weapons.ForEach(x => x.OnUpdate());
	}

	private void OnItemBought(ShopItemData item, int soldPrice)
	{
		if (item.Equipment.IsEquipped)
			item.Equipment.OnUpgrade();
		else
		{
			_weapons.Add(item.Equipment);
			item.Equipment.OnEquip(_identity);
		}
	}

	public void Load(object data)
	{
		SaveData saveData = (SaveData)data;
		Equipment[] equipments = Resources.LoadAll<Equipment>("Data/Equipments");

		foreach (Equipment item in equipments)
		{
			KeyValuePair<string, int> pair = saveData.EquipmentIdUpgradePairs.FirstOrDefault(x => x.Key == item.ID);

			if (!pair.Equals(default(KeyValuePair<string, int>)))
			{
				if (!_weapons.Contains(item))
					_weapons.Add(item);
				item.OnEquip(_identity);
				for (int i = 0; i < pair.Value; i++)
					item.OnUpgrade();
			}
		}
	}

	public object Save() => new SaveData()
	{
		EquipmentIdUpgradePairs = _weapons.Select(x => new KeyValuePair<string, int>(x.ID, x.CurrentUpgrade)).ToArray()
	};
}
