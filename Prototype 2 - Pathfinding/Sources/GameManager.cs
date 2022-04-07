using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private GameObject _howToPlayMenu;
	[SerializeField] private GameObject _endScreen;

	[Header("Resources")]
	[SerializeField] private int _baseWood;
	[SerializeField] private int _baseMetal;

	private SpawnSystem _spawnSystem;

	public int WoodAmount { get; set; }
	public int MetalAmount { get; set; }
	public King King { get; private set; }

	#region Unity builtins

	private void Awake()
	{
		_spawnSystem = GetComponent<SpawnSystem>();
		WoodAmount = _baseWood;
		MetalAmount = _baseMetal;
		King = GetComponentInChildren<King>();
	}

	private void OnEnable()
	{
		Soldier.OnEntityBuildStart += Soldier_OnEntityBuildStart;
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

	private void OnDisable()
	{
		Soldier.OnEntityBuildStart -= Soldier_OnEntityBuildStart;
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
	}

	private void Update()
	{
		if (!_howToPlayMenu.activeSelf) return;

		if (Input.GetKeyDown(KeyCode.Return))
		{
			_howToPlayMenu.SetActive(false);
			StartGame();
		}
	}

	#endregion

	private async void StartGame()
	{
		while (!EntityMap.Instance.Generated)
			await Task.Yield();
		TurnBasedManager.Instance.StartSystem();
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		_howToPlayMenu.SetActive(true);
		iTween.PunchPosition(_howToPlayMenu, Vector3.one * 0.2f, 0.5f);
	}

	private void Soldier_OnEntityBuildStart(EntityData build)
	{
		WoodAmount -= build.IsBuild.WoodCost;
		MetalAmount -= build.IsBuild.MetalCost;
	}

	public void OnFullTurnBegin(int turnNumber)
	{
		if (turnNumber == 0)
			_spawnSystem.InitSpawnAreas();
		_spawnSystem.SpawnResources();
		_spawnSystem.SpawnEnemies(turnNumber);
	}

	public void RetryBtn() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

	public async void OnKingDeath()
	{
		float waitTime = 3f;
		float end = Time.time + waitTime;

		Camera.main.GetComponent<CameraController>().Focus(gameObject);
		Camera.main.GetComponent<CameraController>().Lock();
		while (Time.time < end)
			await Task.Yield();
		_endScreen.GetComponentInChildren<Text>().text = $"You survived {TurnBasedManager.Instance.TurnNumber} turns !";
		_endScreen.SetActive(true);
		TurnBasedManager.Instance.StopSystem();
	}
}
