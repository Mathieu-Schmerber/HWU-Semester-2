using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
	[Title("Room system")]
	[SerializeField] private GameObject _roomPrefab;
	[SerializeField] private Vector2Int _roomNumber;
	[SerializeField] private GameObject[] _enemyPrefabs;
	public Vector2Int MinRoomCellSize;

	[Title("Grid")]
	[SerializeField] private bool _drawGismos = false;
	public Vector2 CellSize;

	private List<RoomNode> _rooms = new List<RoomNode>();

	private void Start() => GenerateStage();

	#region Generation

	private List<RoomNode> GenerateRoomLayout(int minRoom, int maxRoom)
	{
		List<RoomNode> res = new List<RoomNode>();
		List<RoomNode> add = new List<RoomNode>();
		float iterationCutChance = 1;
		int i = 0;

		if (MinRoomCellSize.x <= 0 || MinRoomCellSize.y <= 0) 
			return res;
		res.Add(new RoomNode(new Vector2Int(0, 0), new Vector2Int(MinRoomCellSize.x * (int)Mathf.Sqrt(maxRoom), MinRoomCellSize.y * (int)Mathf.Sqrt(maxRoom))));
		do
		{
			res.AddRange(add);
			add.Clear();
			foreach (RoomNode node in res)
			{
				float cutChances = (res.Count + add.Count < minRoom ? 1 : iterationCutChance);
				int minCut = i % 2 == 0 ? MinRoomCellSize.x : MinRoomCellSize.y;
				int maxCut = i % 2 == 0 ? node.Size.x - MinRoomCellSize.x : node.Size.y - MinRoomCellSize.y;

				// Can we cut a large enough room ?
				// According to random, should a room be split ?
				if (maxCut < minCut || Random.Range(0f, 1f) > cutChances)
					continue;
				add.Add(i % 2 == 0 ?
					node.CutX(Random.Range(minCut, maxCut + 1)) :
					node.CutY(Random.Range(minCut, maxCut + 1))
				);
			}
			iterationCutChance *= 0.85f;
			i++;
		} while (add.Count > 0);
		return res;
	}


	[Button]
	public void GenerateStage()
	{
		Vector2Int innerRoomPadding = Vector2Int.one;

		_rooms = GenerateRoomLayout(_roomNumber.x, _roomNumber.y);
		_rooms.ForEach(node => node.SetLinks(_rooms.Where(other => other != node && other.IsNeigbour(node, innerRoomPadding)), innerRoomPadding));
		foreach (RoomNode node in _rooms)
		{
			Vector3 worldPos = (node.Position * CellSize).ToVector3XZ();
			GameObject roomInstance = Instantiate(_roomPrefab, worldPos, transform.rotation, transform);
			Room room = roomInstance.GetComponent<Room>();

			room.Init(node, node.Size);
		}
		//transform.Rotate(new Vector3(0, 135, 0));
	}

	#endregion

	public GameObject GetRandomEnemyPrefab() => _enemyPrefabs.Random();

	public void DrawRoomGizmosGrid(RoomNode node)
	{
		for (int x = 0; x < node.Size.x; x++) 
			for (int y = 0; y < node.Size.y; y++)
				Gizmos.DrawWireCube(new Vector3(x * CellSize.x, 0, y * CellSize.y), CellSize.ToVector3XZ(0));
	}

	private void OnDrawGizmos()
	{
		if (!_drawGismos) return;

		foreach (RoomNode item in _rooms)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(item.Center.ToVector3XZ(), item.Size.ToVector3XZ());
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(item.Center.ToVector3XZ(), (item.Size - (Vector2Int.one * 2)).ToVector3XZ());
			Gizmos.color = Color.red;
			foreach (RoomNode.Link link in item.Links)
				Gizmos.DrawCube(link.ExitPosition.ToVector3XZ(), Vector3.one * 0.2f);
		}
	}
}