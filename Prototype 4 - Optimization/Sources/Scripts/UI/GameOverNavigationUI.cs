using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverNavigationUI : MonoBehaviour, INavigationUI
{
	[SerializeField] private Button _restartBtn;
	[SerializeField] private Button _shopBtn;
	[SerializeField] private Button _backBtn;
	[SerializeField] private StatLineUI _killStat;
	[SerializeField] private StatLineUI _gainStat;
	[SerializeField] private Text _coinsAmountTxt;

	private AudioSource _source;
	private CanvasGroup _canvasGroup;
	private RectTransform _rect;
	private MenuManagement _menuManagement;

	private void Awake()
	{
		_source = GetComponent<AudioSource>();
		_canvasGroup = GetComponent<CanvasGroup>();
		_rect = GetComponent<RectTransform>();
		_menuManagement = GetComponentInParent<MenuManagement>();
		_canvasGroup.interactable = true;
	}

	private void Start()
	{
		_restartBtn.onClick.AddListener(() => _menuManagement.ShowInGame());
		_shopBtn.onClick.AddListener(() => _menuManagement.ShowShop());
		_backBtn.onClick.AddListener(() => _menuManagement.ShowMainMenu());
	}

	private void OnCoinsAmountChanged(int amount)
	{
		_coinsAmountTxt.text = $"{amount}¤";
		Tween.LocalScale(_coinsAmountTxt.transform, Vector3.one * 1.4f, 0.1f, 0);
		Tween.LocalScale(_coinsAmountTxt.transform, Vector3.one, 0.1f, 0.1f);
		_source.Play();
	}

	public void Close()
	{
		if (!gameObject.activeSelf) return;

		Vector3 destination = -Vector3.up * Screen.height;

		_canvasGroup.interactable = false;
		Tween.CanvasGroupAlpha(_canvasGroup, 0, 0.3f, 0, Tween.EaseIn, completeCallback: () => gameObject.SetActive(false));
		Tween.AnchoredPosition(_rect, Vector3.zero, destination, 0.3f, 0, Tween.EaseIn);
	}

	public void Open()
	{
		gameObject.SetActive(true);
		_canvasGroup.interactable = true;
		Tween.CanvasGroupAlpha(_canvasGroup, 1, 0.3f, 0, Tween.EaseOut);
		Tween.AnchoredPosition(_rect, Vector3.zero, 0.3f, 0, Tween.EaseOut);

		float delay = .3f;

		_coinsAmountTxt.text = $"{GameStats.Instance.Coins}¤";
		_killStat.SetValue("0");
		_gainStat.SetValue("+0¤");

		_killStat.Show(delay);
		delay += 0.3f;
		_gainStat.Show(delay);
		delay *= 2;

		Tween.Value(0, GameStats.Instance.Kills, (value) => _killStat.SetValue(value.ToString()), 1f, delay);
		Tween.Value(0, GameStats.Instance.CoinGain, (value) => { 
			_gainStat.SetValue($"+{value}¤"); 
			_source.Play();
		}, 1f, delay);
		Tween.Value(GameStats.Instance.Coins, GameStats.Instance.Coins + GameStats.Instance.CoinGain, (value) => OnCoinsAmountChanged(value), .5f, delay + 1.5f);
		GameStats.Instance.CashGain();
	}
}
