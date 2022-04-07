using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MenuManagement : MonoBehaviour
{
	[SerializeField] private Image _background;
	[SerializeField] private Volume _postprocessing;

	private DepthOfField _dof;
	private Color _bgColor;
	private Color _transparent = new Color(0, 0, 0, 0);
    private MainMenuNavigationUI _mainMenu;
    private ShopNavigationUI _shop;
	private InGameNavigationUI _inGame;
	private GameOverNavigationUI _gameOver;
	private StatNavigationUI _stats;

	#region Unity builtins

	private void OnEnable()
	{
		GameLoop.OnGameEnded += OnGameEnded;
	}

	private void OnDisable()
	{
		GameLoop.OnGameEnded -= OnGameEnded;
	}

	#endregion

	private void Awake()
	{
		_mainMenu = GetComponentInChildren<MainMenuNavigationUI>(true);
		_shop = GetComponentInChildren<ShopNavigationUI>(true);
		_inGame = GetComponentInChildren<InGameNavigationUI>(true);
		_gameOver = GetComponentInChildren<GameOverNavigationUI>(true);
		_stats = GetComponentInChildren<StatNavigationUI>(true);
		_bgColor = _background.color;

		_postprocessing.profile.TryGet(out _dof);
		_dof.focusDistance.value = 0;
	}

	private void Start()
	{
		_shop.Close();
		_inGame.Close();
		_gameOver.Close();
		_stats.Close();
		ShowMainMenu();
		AudioManager.SetPitch(0.8f, 0.5f);
		AudioManager.SetVolume(0.8f, 0.5f);
	}

	public void ShowInGame()
	{
		GameLoop.Camera.ResetZoom();
		AudioManager.SetVolume(1f, 0.5f);
		AudioManager.SetPitch(1f, 0.5f);
		_mainMenu.Close();
		_gameOver.Close();
		_inGame.Open();
		Tween.Value(_bgColor, _transparent, (color) => _background.color = color, 0.2f, 0, Tween.EaseIn);
		Tween.Value(0f, GameLoop.Camera.BaseFov, (blur) => _dof.focusDistance.value = blur, 0.2f, 0, Tween.EaseIn);
	}

	private void OnGameEnded()
	{
		_inGame.Close();
		_gameOver.Open();
		Tween.Value(_transparent, _bgColor, (color) => _background.color = color, 0.2f, 0, Tween.EaseIn);
		Tween.Value(GameLoop.Camera.BaseFov, 0f, (blur) => _dof.focusDistance.value = blur, 0.2f, 0, Tween.EaseIn);
	}

	public void ShowMainMenu()
	{
		_mainMenu.Open();
		_stats.Close();
		_gameOver.Close();
		_shop.Close();
	}

	public void ShowShop()
	{
		_mainMenu.Close();
		_gameOver.Close();
		_shop.Open();
	}

	public void ShowStats()
	{
		_mainMenu.Close();
		_stats.Open();
	}
}
