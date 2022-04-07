using Nawlian.Lib.Systems.Saving;
using System;
using UnityEngine;

public class GameLoop : Singleton<GameLoop>
{
	private bool _inGame;
	[SerializeField] private EntityIdentity _player;
	[SerializeField] private CameraFollow _camera;

	public static EntityIdentity Player => Instance._player;
	public static CameraFollow Camera => Instance._camera;
	public static bool HasGameStarted => Instance._inGame;

	public static event Action OnGameStarted;
	public static event Action OnGameEnded;

	private void Start() => SaveSystem.Load();

	public static void StartGame()
	{
		if (!HasGameStarted)
		{
			Camera.ResetZoom();
			Instance._inGame = true;
			OnGameStarted?.Invoke();
			Player.CurrentHealth = (int)Player.MaxHealth.Value;
		}
	}

	public static void EndGame()
	{
		if (HasGameStarted)
		{
			Instance._inGame = false;
			OnGameEnded?.Invoke();
			SaveSystem.Save();
		}
	}
}