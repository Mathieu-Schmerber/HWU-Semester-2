using Nawlian.Lib.Systems.Saving;
using Nawlian.Lib.Utils;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	#region Types

	[System.Serializable]
	public struct EnemyComposite
	{
		public EnemyData Enemy;
		public AnimationCurve SpawnChance;
	}

	#endregion

	[SerializeField] private EnemyComposite[] _enemies;
	[SerializeField] private float _spawnRange = 20f;
	[SerializeField] private float _waveTime = 60f;

	private Timer _waveTimer = new Timer();
	private Timer _spawnTimer = new Timer();
	private int _waveNumber = 1;

	/// f(x) = x/6 + 0.8
	private float _difficultyFactor => _waveNumber / 6f + .8f;
	/// f(x) = x² + 15x + 10
	private float _wavePopulation => (int)Mathf.Pow(_waveNumber, 2) + 15 * _waveNumber + 10;
	private float _spawnRate => _waveTime / _wavePopulation;

	public static event System.Action<int> OnNewWaveStarted;

	#region Unity builtins

	private void OnEnable()
	{
		GameLoop.OnGameStarted += OnGameStart;
		GameLoop.OnGameEnded += OnGameEnd;
	}

	private void OnDisable()
	{
		GameLoop.OnGameStarted -= OnGameStart;
		GameLoop.OnGameEnded -= OnGameEnd;
	}

	#endregion

	private void OnGameStart()
	{
		// OnWaveTick() will increase by 1, we're starting with one wave lower than our max
		_waveNumber = GameStats.Instance.MaxLevel - 2 < 1 ? 0 : GameStats.Instance.MaxLevel - 2; 
		OnWaveTick();
		_waveTimer.Start(_waveTime, true, OnWaveTick);
		_spawnTimer.Start(_spawnRate, true, OnSpawnTick);
	}

	private void OnGameEnd()
	{
		_waveTimer.Stop();
		_spawnTimer.Stop();
		if (GameStats.Instance.MaxLevel < _waveNumber)
			GameStats.Instance.MaxLevel = _waveNumber;
	}

	private Vector3 GetPositionAroundPlayer(float spawnRange)
	{
		Vector3 rdm = Random.insideUnitCircle;

		while (rdm == Vector3.zero)
			rdm = Random.insideUnitCircle;
		return GameLoop.Player.transform.position + (rdm.normalized * spawnRange);
	}

	private EnemyComposite GetRandom()
	{
		float u = _enemies.Sum(p => p.SpawnChance.Evaluate(_waveNumber));
		float r = Random.Range(0f, u);
		float sum = 0;

		foreach (EnemyComposite n in _enemies)
		{
			if (r <= (sum = sum + n.SpawnChance.Evaluate(_waveNumber)))
				return n;
		}
		return default(EnemyComposite);
	}

	private void OnSpawnTick()
	{
		EnemyComposite composite = GetRandom();

		if (composite.Equals(default(EnemyComposite)))
			return;
		EnemyPool.Spawn(GetPositionAroundPlayer(_spawnRange), composite.Enemy, _difficultyFactor);
	}

	private void OnWaveTick()
	{
		_waveNumber++;
		_spawnTimer.Interval = _spawnRate;
		_spawnTimer.Restart();
		OnNewWaveStarted?.Invoke(_waveNumber);
	}
}