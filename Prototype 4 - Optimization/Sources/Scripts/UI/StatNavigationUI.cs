using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Saving;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatNavigationUI : MonoBehaviour, INavigationUI
{
	[SerializeField] private StatLineUI _totalKills;
	[SerializeField] private StatLineUI _totalDeaths;
	[SerializeField] private StatLineUI _maxLevel;
	[SerializeField] private Button _resetBtn;
	[SerializeField] private Button _backBtn;

	private CanvasGroup _canvasGroup;
	private RectTransform _rect;
	private MenuManagement _menu;

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_rect = GetComponent<RectTransform>();
		_menu = GetComponentInParent<MenuManagement>();
	}

	private void Start()
	{
		_resetBtn.onClick.AddListener(() => {
			SaveSystem.Wipe();
			Resources.LoadAll<Equipment>("").ForEach(x => x.Reset());
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		});
		_backBtn.onClick.AddListener(_menu.ShowMainMenu);
	}

	private void Refresh()
	{
		_totalKills.SetValue("0");
		_totalDeaths.SetValue("0");
		_maxLevel.SetValue("0");

		_totalKills.Show(0);
		_totalDeaths.Show(.1f);
		_maxLevel.Show(.2f);

		Tween.Value(0, GameStats.Instance.TotalKills, (value) => _totalKills.SetValue(value.ToString()), 1f, .3f);
		Tween.Value(0, GameStats.Instance.Deaths, (value) => _totalDeaths.SetValue(value.ToString()), 1f, .3f);
		Tween.Value(0, GameStats.Instance.MaxLevel, (value) => _maxLevel.SetValue(value.ToString()), 1f, .3f);
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

		Refresh();
	}
}
