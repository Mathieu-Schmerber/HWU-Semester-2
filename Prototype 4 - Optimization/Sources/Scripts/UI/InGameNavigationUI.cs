using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameNavigationUI : MonoBehaviour, INavigationUI
{
	private CanvasGroup _group;
	private PlayerController _player;
	[SerializeField] private CanvasGroup _tutorialPanel;
	[SerializeField] private Text _killText;
	[SerializeField] private Text _levelText;

	private void Awake()
	{
		_group = GetComponent<CanvasGroup>();
		_player = GameLoop.Player.GetComponent<PlayerController>();
	}

	private void OnEnable()
	{
		GameStats.Instance.OnKillsUpdated += Instance_OnKillsUpdated;
		EnemySpawner.OnNewWaveStarted += OnWaveStarted;
	}

	private void OnDisable()
	{
		if (GameStats.Instance != null)
			GameStats.Instance.OnKillsUpdated -= Instance_OnKillsUpdated;
		EnemySpawner.OnNewWaveStarted -= OnWaveStarted;
	}

	public void OnLeftDown()
	{
		if (!_group.interactable) return;
		else if (!GameLoop.HasGameStarted)
		{
			_killText.gameObject.SetActive(true);
			_levelText.gameObject.SetActive(true);
			_tutorialPanel.interactable = false;
			Tween.CanvasGroupAlpha(_tutorialPanel, 0, 0.3f, 0, Tween.EaseOut);
			GameLoop.StartGame();
		}
		_player.TurnOn(PlayerController.Reactor.LEFT);
	}

	public void OnLeftUp() => _player.TurnOff(PlayerController.Reactor.LEFT);
	public void OnRightDown()
	{
		if (!_group.interactable) return;
		else if (!GameLoop.HasGameStarted)
		{
			_killText.gameObject.SetActive(true);
			_levelText.gameObject.SetActive(true);
			_tutorialPanel.interactable = false;
			Tween.CanvasGroupAlpha(_tutorialPanel, 0, 0.3f, 0, Tween.EaseOut);
			GameLoop.StartGame();
		}
		_player.TurnOn(PlayerController.Reactor.RIGHT);
	}
	public void OnRightUp() => _player.TurnOff(PlayerController.Reactor.RIGHT);

	private void Instance_OnKillsUpdated(int obj)
	{
		_killText.text = $"{obj} Kills";
		Tween.LocalScale(_killText.transform, Vector3.one * 0.8f, Vector3.one, 0.2f, 0, Tween.EaseBounce);
		Tween.Value(Color.white, Color.gray, (color) => _killText.color = color, 0.3f, 0, Tween.EaseOut);
	}
	private void OnWaveStarted(int waveNumber)
	{
		_levelText.text = $"LEVEL {waveNumber}";
		Tween.LocalScale(_levelText.transform, Vector3.one * 0.8f, Vector3.one, 0.2f, 0, Tween.EaseBounce);
		Tween.Value(Color.white, Color.gray, (color) => _levelText.color = color, 0.3f, 0, Tween.EaseOut);
	}

	public void Open()
	{
		_group.alpha = 1;
		_group.interactable = true;
		_tutorialPanel.alpha = 1;
		_killText.gameObject.SetActive(false);
		_levelText.gameObject.SetActive(false);
		gameObject.SetActive(true);
	}

	public void Close()
	{
		if (!gameObject.activeSelf) return;

		_group.alpha = 0;
		_group.interactable = false;
		_tutorialPanel.alpha = 0;
		_killText.gameObject.SetActive(false);
		_levelText.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
}
