using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
	[SerializeField] private bool _drawGizmos;

	[Header("Resources Spawn")]
	[SerializeField] private GameObject _treePrefab;
	[SerializeField] private GameObject _rockPrefab;
	[SerializeField] private Bounds _resourceSpawnArea;
	[SerializeField] private Bounds _resourceBanArea;
	[SerializeField, Range(0, 100)] private int _rockRatio;
	[SerializeField, Range(0, 100)] private int _treeRatio;

	[Header("Enemies")]
	[SerializeField] private int _turnSpawnInterval = 1;
	[SerializeField] private GameObject[] _enemies;
	[SerializeField] private Bounds[] _enemySpawnAreas;

	private List<Node> _enemySpawnableNodes = new List<Node>();
	private List<Node> _resourcesSpawnableNodes = null;
	private List<GameObject> _trees = new List<GameObject>();
	private List<GameObject> _rocks = new List<GameObject>();

	private void Start()
	{
		
	}

	public void InitSpawnAreas()
	{
		_resourcesSpawnableNodes = EntityMap.Instance.GetNodes(_resourceSpawnArea);
		_resourcesSpawnableNodes.RemoveAll(x => _resourceBanArea.Contains(x.WorldPos));

		foreach (Bounds bounds in _enemySpawnAreas)
			_enemySpawnableNodes.AddRange(EntityMap.Instance.GetNodes(bounds).Where(x => !_enemySpawnableNodes.Contains(x)));
		_enemySpawnableNodes.RemoveAll(x => !x.IsWalkable);
	}

	#region Enemies spawn

	public async void SpawnEnemies(int turnNumber)
	{
		if (turnNumber % _turnSpawnInterval != 0 || !enabled) return;

		int x = turnNumber / _turnSpawnInterval;
		int numberToSpawn = Mathf.FloorToInt(0.02f * Mathf.Pow(x, 2) + x + 1); // f(x) = 0.02x² + x + 1
		List<Node> spawnables = new List<Node>(_enemySpawnableNodes);

		spawnables.RemoveAll(x => !x.IsWalkable);
		for (int i = 0; i < numberToSpawn; i++)
		{
			float end = Time.time + 0.1f;
			while (Time.time < end)
				await Task.Yield();

			Node node = spawnables.Random();
			EntityMap.Instance.SpawnEntity(_enemies.Random(), node);
			spawnables.Remove(node);
		}
	}

	#endregion

	#region Resources spawn

	/// <summary>
	/// Spawn a define number of entities
	/// </summary>
	/// <param name="prefab"></param>
	/// <param name="number"></param>
	/// <param name="trackingList"></param>
	private void SpawnNumber(GameObject prefab, int number, List<GameObject> collection)
	{
		List<Node> spawnables = new List<Node>(_resourcesSpawnableNodes);

		spawnables.RemoveAll(x => !x.IsEmpty);
		for (int i = 0; i < number; i++)
		{
			Node node = spawnables.Random();
			collection.Add(EntityMap.Instance.SpawnEntity(prefab, node));
			spawnables.Remove(node);
		}
	}

	/// <summary>
	/// Spawn resources by ratio
	/// </summary>
	/// <param name="prefab"></param>
	/// <param name="ratio"></param>
	/// <param name="trackingList"></param>
	private void SpawnResourceType(GameObject prefab, int ratio, List<GameObject> collection)
	{
		List<Node> spawnables = new List<Node>(_resourcesSpawnableNodes);

		spawnables.RemoveAll(x => !x.IsEmpty);
		float population = spawnables.Count * (ratio / 100f);

		for (int i = 0; i < population; i++)
		{
			Node node = spawnables.Random();
			collection.Add(EntityMap.Instance.SpawnEntity(prefab, node));
			spawnables.Remove(node);
		}
	}

	/// <summary>
	/// Spawn all entities that can drop reosurces
	/// </summary>
	public async void SpawnResources()
	{
		if (!enabled) return;

		// Spawn the resources by ratio on the first turn
		if (_rocks.Count == 0)
			SpawnResourceType(_rockPrefab, _rockRatio, _rocks);
		if (_trees.Count == 0)
			SpawnResourceType(_treePrefab, _treeRatio, _trees);
		// Refill trees and rocks based on what was destroyed
		if (_rocks.Count(x => x == null) > 0)
			SpawnNumber(_rockPrefab, _rocks.Count(x => x == null), _rocks);
		if (_trees.Count(x => x == null) > 0)
			SpawnNumber(_treePrefab, _trees.Count(x => x == null), _trees);

		_rocks.RemoveAll(x => x == null);
		_trees.RemoveAll(x => x == null);
		await Task.Yield();
	}

	#endregion

	private void OnDrawGizmos()
	{
		if (!enabled || !_drawGizmos) return;

		Gizmos.color = new Color(0, 255, 0, 0.3f);
		Gizmos.DrawCube(_resourceSpawnArea.center + Vector3.up * 0.1f, _resourceSpawnArea.size);

		Gizmos.color = new Color(255, 0, 0, 0.3f);
		Gizmos.DrawCube(_resourceBanArea.center + Vector3.up * 0.1f, _resourceBanArea.size);

		Gizmos.color = new Color(255, 0, 0, 0.5f);

		foreach (Bounds bounds in _enemySpawnAreas)
			Gizmos.DrawCube(bounds.center + Vector3.up * 0.1f, bounds.size);
	}
}
