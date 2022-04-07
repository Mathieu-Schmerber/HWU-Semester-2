using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameEndEventArgs : System.EventArgs
{
	public int SheepCount { get; set; }
	public int SheepKilled { get; set; }
	public int SheepEaten { get; set; }
	public Gamemode.Mode PlayedMode { get; set; }
}

public class Spawner : Singleton<Spawner>
{
	[Header("Prefabs")]
	[SerializeField] private GameObject _sheepPrefab;
	[SerializeField] private GameObject _hunterPrefab;
	[SerializeField] private GameObject _wolfPrefab;

	[Header("Sheeps")]
	[SerializeField] private int _sheepQuantity;
	[SerializeField] private Bounds _spawnArea;
	[SerializeField] private bool _drawGizmos = true;

	private GameObject _hunterInstance;
	private GameObject _wolfInstance;
	private readonly List<SheepAI> _sheeps = new List<SheepAI>();
	private DetectionLight[] _detectionLights;

	public delegate void OnGameEnd(GameEndEventArgs gameStats);
	public static event OnGameEnd OnGameEndsEvt;

	private bool _gameRunning = false;
	public bool GameRunning => _gameRunning;

	#region Unity builtins

	private void Awake()
	{
		CanvasManager.OnUserStartsGameEvt += StartGame;
	}

	private void Start()
	{
		_detectionLights = GameObject.FindObjectsOfType<DetectionLight>().Where(x => x.gameObject.isStatic).ToArray();
	}

	private void OnDestroy()
	{
		CanvasManager.OnUserStartsGameEvt -= StartGame;
	}

	private void OnDrawGizmos()
	{
		if (_drawGizmos)
		{
			Gizmos.color = new Color(255, 0, 0);
			Gizmos.DrawWireCube(_spawnArea.center, _spawnArea.size);
		}
	}

	#endregion

	#region Game start and end

	private void SpawnSheeps()
	{
		for (int i = 0; i < _sheepQuantity; i++)
		{
			Vector3 pos = new Vector3(Random.Range(_spawnArea.min.x, _spawnArea.max.x), _spawnArea.center.y, Random.Range(_spawnArea.min.z, _spawnArea.max.z));
			SheepAI sheep = Instantiate(_sheepPrefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<SheepAI>();

			_sheeps.Add(sheep);
		}
	}

	private GameObject SpawnHunter()
	{
		Vector3 position = GetRandomPositionAroundLight(0);

		return Instantiate(_hunterPrefab, position, Quaternion.identity);
	}

	private GameObject SpawnWolf()
	{
		Vector3 position = GetRandomPositionInShadows();

		return Instantiate(_wolfPrefab, position, Quaternion.identity);
	}

	private void StartGame(Gamemode.Mode mode)
	{
		SpawnSheeps();
		_hunterInstance = SpawnHunter();
		_wolfInstance = SpawnWolf();
		Gamemode.Instance.CurrentMode = mode;
		_gameRunning = true;
	}

	private void EndGame()
	{
		OnGameEndsEvt?.Invoke(new GameEndEventArgs()
		{
			SheepCount = _sheeps.Count,
			SheepKilled = _sheepQuantity - _sheeps.Count,
			PlayedMode = Gamemode.Instance.CurrentMode
		});

		if (_sheeps.Count > 0)
			_sheeps.ForEach(x => Destroy(x.gameObject));

		_gameRunning = false;
		Destroy(_hunterInstance);
		Destroy(_wolfInstance);
		_sheeps.Clear();
	}

	#endregion

	#region Win conditions

	public void KillWolf(WolfEmitter wolf)
	{
		Destroy(wolf.gameObject);
		EndGame();
	}

	public void KillSheep(SheepAI sheep)
	{
		_sheeps.Remove(sheep);
		Destroy(sheep.gameObject);

		if (_sheeps.Count == 0)
			EndGame();
	}

	#endregion

	#region Tools

	public bool IsPositionOnMap(Vector3 position) => _spawnArea.Contains(new Vector3(position.x, _spawnArea.min.y, position.z));
	public bool IsPositionInLight(Vector3 position)
	{
		foreach (var light in _detectionLights)
		{
			if (Vector3.Distance(position, light.transform.position) <= light.Radius)
				return true;
		}
		return false;
	}

	public Transform GetClosestSheep(Vector3 position) =>
		_sheeps.Aggregate((s1, s2) => Vector3.Distance(s1.transform.position, position) < Vector3.Distance(s2.transform.position, position) ? s1 : s2).transform;

	public Vector3 GetRandomPosition(float forcedY) => 
		new Vector3(Random.Range(_spawnArea.min.x, _spawnArea.max.x), forcedY, Random.Range(_spawnArea.min.z, _spawnArea.max.z));

	public Vector3 GetRandomPositionInShadows()
	{
		Vector3 result = GetRandomPosition(0);

		while (IsPositionInLight(result))
			result = GetRandomPosition(0);
		return result;
	}

	public Vector3 GetRandomPositionAroundLight(float forcedY)
	{
		DetectionLight rdnLight = _detectionLights[Random.Range(0, _detectionLights.Length)];
		Vector3 rndPos = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);
		Vector3 position = rdnLight.transform.position + rndPos * rdnLight.Radius;

		while (!IsPositionOnMap(position))
		{
			rndPos = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);
			position = rdnLight.transform.position + rndPos * rdnLight.Radius;
		}
		return position;
	}

	#endregion
}
