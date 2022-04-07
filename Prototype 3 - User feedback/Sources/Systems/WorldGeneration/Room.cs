using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private bool _drawGrid = true;
	[SerializeField] private GameObject _groundPrefab;
	[SerializeField] private GameObject _exitPrefab;

	private BoxCollider _roomTrigger;
	private int _mobsToSpawn;
	private List<GameObject> _spawnedEntities = new List<GameObject>();

	private GameObject[] _exits;

	/// <summary>
	/// The walkable area inner the room grid
	/// </summary>
	private Vector2Int _innerSize;

	/// <summary>
	/// The total room's grid
	/// </summary>
	private Vector2Int _gridSize;

	public RoomNode Node { get; private set; }

	public void Init(RoomNode node, Vector2Int size)
	{
		Node = node;
		_gridSize = size;
		_innerSize = _gridSize - Vector2Int.one * 2;
		GenerateRoom();
	}

	#region Generation

	#region Ground

	private GameObject SpawnAndScaleOnGrid(GameObject prefab, Vector2Int gridPos, Vector2Int cellCoverage)
	{
		GameObject instance = Instantiate(prefab, GetCellWorldPos(gridPos), Quaternion.identity, transform);
		Transform child = instance.transform.GetChild(0);

		instance.transform.localScale = new Vector3(StageManager.Instance.CellSize.x, 1, StageManager.Instance.CellSize.y);
		child.localScale = new Vector3(cellCoverage.x, 1, cellCoverage.y);
		child.localPosition = new Vector3(child.localScale.x / 2 - .5f, child.localPosition.y, child.localScale.z / 2 - .5f);
		return instance;
	}

	private Vector3 GetCellWorldPos(Vector2Int gridPos)
		=> transform.position + new Vector3(gridPos.x * StageManager.Instance.CellSize.x, 0, gridPos.y * StageManager.Instance.CellSize.y);

	private void GenerateGround()
	{
		SpawnAndScaleOnGrid(_groundPrefab, Vector2Int.zero, _gridSize);
	}

	#endregion

	#region Buildings

	private int GetRemainingSize(List<Vector2Int> line, Vector2Int currentPos)
	{
		int index = line.IndexOf(currentPos);
		int size = 0;
		Vector2Int prev = currentPos;

		if (index + 1 >= line.Count)
			return 1;
		for (int i = index; i < line.Count; i++)
		{
			if (Vector2Int.Distance(line[i], prev) > 1)
				break;
			size++;
			prev = line[i];
		}
		return size;
	}

	private void SpawnBuildingsOnLine(List<Vector2Int> line, Quaternion rotation, Direction direction)
	{
		BuildingNode prev = null;

		for (int i = 0; i < line.Count;)
		{
			Vector3 worldpos = GetCellWorldPos(line[i]);
			int availableSize = GetRemainingSize(line, line[i]);
			if (availableSize == 0)
			{
				i++;
				continue;
			}

			BuildingNode prefab = Resources.LoadAll<BuildingNode>("Prefabs/RoomBuilds/Buildings").Where(x => x.CellSize <= availableSize && x != prev).ToArray().Random();
			Vector3 offset = Vector3.zero;

			// Offsetting wide buildings
			if (direction == Direction.EAST)
				offset = (prefab.CellSize - 1) * StageManager.Instance.CellSize.x * Vector3.forward;
			else if (direction == Direction.SOUTH)
				offset = (prefab.CellSize - 1) * StageManager.Instance.CellSize.x * Vector3.right;

			GameObject build = Instantiate(prefab.gameObject, worldpos + offset, rotation, transform);

			build.transform.localScale = new Vector3(StageManager.Instance.CellSize.x,
													 Mathf.Max(StageManager.Instance.CellSize.x, StageManager.Instance.CellSize.y),
													 StageManager.Instance.CellSize.y);
			prev = prefab;
			i += prefab.CellSize;
		}
	}

	private void GenerateBuildings()
	{
		List<Vector2Int> northPoses = new List<Vector2Int>();
		List<Vector2Int> southPoses = new List<Vector2Int>();
		List<Vector2Int> westPoses = new List<Vector2Int>();
		List<Vector2Int> eastPoses = new List<Vector2Int>();

		for (int x = 0; x < _gridSize.x; x++)
		{
			if (Node.Exits.Any(pos => pos.x == x && pos.y == 0))
				continue;
			northPoses.Add(new Vector2Int(x, 0));
		}
		for (int x = 0; x < _gridSize.x; x++)
		{
			if (Node.Exits.Any(pos => pos.x == x && pos.y == _gridSize.y - 1))
				continue;
			southPoses.Add(new Vector2Int(x, _gridSize.y) - Vector2Int.up);
		}
		for (int y = 1; y < _gridSize.y - 1; y++)
		{
			if (Node.Exits.Any(pos => pos.x == 0 && pos.y == y))
				continue;
			eastPoses.Add(new Vector2Int(0, y));
		}	
		for (int y = 1; y < _gridSize.y - 1; y++)
		{
			if (Node.Exits.Any(pos => pos.x == _gridSize.x - 1 && pos.y == y))
				continue;
			westPoses.Add(new Vector2Int(_gridSize.x, y) - Vector2Int.right);
		}
		SpawnBuildingsOnLine(northPoses, Quaternion.Euler(0, 0, 0), Direction.NORTH);
		SpawnBuildingsOnLine(southPoses, Quaternion.Euler(0, 180, 0), Direction.SOUTH);
		SpawnBuildingsOnLine(westPoses, Quaternion.Euler(0, -90, 0), Direction.WEST);
		SpawnBuildingsOnLine(eastPoses, Quaternion.Euler(0, 90, 0), Direction.EAST);
	}

	#endregion

	#region Exits

	private void GenerateExitBarriers()
	{
		Vector3[] exitPoses = Node.Exits.Select(x => GetCellWorldPos(x)).ToArray();

		_exits = new GameObject[exitPoses.Length];
		for (int i = 0; i < exitPoses.Length; i++)
		{
			GameObject exitObj = Instantiate(_exitPrefab, exitPoses[i], Quaternion.identity, transform);

			exitObj.transform.localScale = new Vector3(StageManager.Instance.CellSize.x,
													   Mathf.Max(StageManager.Instance.CellSize.x, StageManager.Instance.CellSize.y),
													   StageManager.Instance.CellSize.y);
			exitObj.SetActive(false);
			_exits[i] = exitObj;
		}
	}

	private void GenerateRoomTrigger()
	{
		Vector3 size = Vector3.Scale(StageManager.Instance.CellSize.ToVector3XZ(1), Node.Size.ToVector3XZ(1));

		_roomTrigger = gameObject.AddComponent<BoxCollider>();
		_roomTrigger.isTrigger = true;
		_roomTrigger.size = size - Vector3.Scale(Vector3.one.WithY(0), StageManager.Instance.CellSize.ToVector3XZ(0)) * 3;
		_roomTrigger.center = (size * 0.5f) - new Vector3(StageManager.Instance.CellSize.x / 2, 0, StageManager.Instance.CellSize.y / 2);
	}

	#endregion

	public void GenerateRoom()
	{
		GenerateGround();
		GenerateBuildings();
		GenerateExitBarriers();
		GenerateRoomTrigger();

		_mobsToSpawn = (Node.Size.x + Node.Size.y) / 2;
	}

	#endregion

	private void OnEnemyDeath(GameObject entity)
	{
		_spawnedEntities.Remove(entity);
		if (_spawnedEntities.Count == 0)
			_exits.ForEach(x => x.gameObject.SetActive(false));
	}

	private void SpawnEnemies(int amount)
	{
		Bounds roomBounds = new Bounds(transform.position + _roomTrigger.center, _roomTrigger.size);

		for (int i = 0; i < amount; i++)
		{
			Vector3 rdmPos = roomBounds.GetRandomPoint().WithY(1);
			GameObject entity = Instantiate(StageManager.Instance.GetRandomEnemyPrefab(), rdmPos, Quaternion.identity);

			_spawnedEntities.Add(entity);
			entity.GetComponent<Damageable>().OnDeath += () => OnEnemyDeath(entity);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// TODO: get the player thanks to some manager instead
		if (other.GetComponent<PlayerController>() == null || _mobsToSpawn == 0) return;

		_exits.ForEach(x => x.gameObject.SetActive(true));
		SpawnEnemies(_mobsToSpawn);
		_mobsToSpawn = 0;
	}

	private void OnDrawGizmos()
	{
		if (!_drawGrid) return;

		Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.lossyScale);
		Gizmos.matrix = rotationMatrix;
		StageManager.Instance.DrawRoomGizmosGrid(Node);

		foreach (Vector2Int link in Node.Exits)
			Gizmos.DrawCube(GetCellWorldPos(link), new Vector3(StageManager.Instance.CellSize.x, 0, StageManager.Instance.CellSize.y));
	}
}
