using Nawlian.Lib.Systems.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : Singleton<GameStats>, ISaveable
{
	[System.Serializable]
	private struct SaveData
	{
		public int TotalKills;
		public int Deaths;
		public int MaxLevel;
		public int Coins;
	}

	private int _coins;
	private int _kills;

	public event Action<int> OnCoinsUpdated;
	public event Action<int> OnKillsUpdated;

	public int Coins {
		get => _coins;
		private set {
			_coins = value;
			OnCoinsUpdated?.Invoke(value);
		} 
	}

	public int Kills
	{
		get => _kills;
		private set
		{
			_kills = value;
			OnKillsUpdated?.Invoke(value);
		}
	}

	public int CoinGain { get; private set; }
	public int MaxLevel { get; set; }
	public int TotalKills { get; set; }
	public int Deaths { get; set; }

	#region Unity builtins

	private void OnEnable()
	{
		ShopNavigationUI.OnItemBought += ShopNavigationUI_OnItemBought;
		GameLoop.OnGameStarted += GameLoop_OnGameStarted;
		Damageable.OnDeath += OnEntityKilled;
	}

	private void OnDisable()
	{
		ShopNavigationUI.OnItemBought -= ShopNavigationUI_OnItemBought;
		GameLoop.OnGameStarted -= GameLoop_OnGameStarted;
		Damageable.OnDeath -= OnEntityKilled;
	}

	#endregion

	public void CashGain()
	{
		Coins += CoinGain;
	}

	private void GameLoop_OnGameStarted()
	{
		CoinGain = 0;
		Kills = 0;
	}

	private void ShopNavigationUI_OnItemBought(ShopItemData item, int soldPrice)
	{
		Coins -= soldPrice;
	}

	private void OnEntityKilled(EntityIdentity obj)
	{
		if (obj.IsPlayer)
			Deaths++;
		else
		{
			TotalKills++;
			Kills++;
			CoinGain += obj.CoinWorth;
		}
	}

	public void Load(object data)
	{
		SaveData saveData = (SaveData)data;

		Coins = saveData.Coins;
		TotalKills = saveData.TotalKills;
		Deaths = saveData.Deaths;
		MaxLevel = saveData.MaxLevel;
		Coins = saveData.Coins;
	}

	public object Save() => new SaveData() {
		TotalKills = TotalKills,
		Deaths = Deaths,
		MaxLevel = MaxLevel,
		Coins = _coins 
	};
}