using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using System.Diagnostics;

public class EntityMap : Singleton<EntityMap>
{
    [SerializeField] private GameObject _ground;
    [SerializeField] private Transform _entityStorage;
    [SerializeField] private float _gridAltitude = 1;

    private Vector3 _worldNodeOffset;
    private Node[,] _graph;
    private int _mapSizeX;
    private int _mapSizeY;

	public bool Generated { get; private set; }

	#region Load

    void Start()
    {
        Generated = false;

        //ensures the size of the graph is always the same as the size of the ground object
        _mapSizeX = (int)_ground.transform.localScale.x;
        _mapSizeY = (int)_ground.transform.localScale.z;
        _worldNodeOffset = new Vector3(-_mapSizeX / 2, _gridAltitude, -_mapSizeY / 2);
        InstantiateGraph();
        AddCurrentEntitiesToGraph();

        Generated = true;
    }

    private void InstantiateGraph()
    {
        _graph = new Node[_mapSizeX, _mapSizeY];
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                _graph[x, y] = new Node();
                _graph[x, y].Position = new Vector2Int(x, y);
            }
        }

        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                if (x > 0)
                {
                    _graph[x, y].Neighbors.Add(_graph[x - 1, y]);
                }
                if (x < _mapSizeX - 1)
                {
                    _graph[x, y].Neighbors.Add(_graph[x + 1, y]);
                }
                if (y > 0)
                {
                    _graph[x, y].Neighbors.Add(_graph[x, y - 1]);
                }
                if (y < _mapSizeY - 1)
                {
                    _graph[x, y].Neighbors.Add(_graph[x, y + 1]);
                }
            }
        }
    }

    /// <summary>
    /// Scans the board, and add every EntityType already on it
    /// </summary>
    private void AddCurrentEntitiesToGraph()
    {
        EntityType[] entities = GameObject.FindObjectsOfType<EntityType>();

        foreach (EntityType entity in entities)
        {
            Node node = GetNode(entity.transform.position);

            if (node == null) 
                continue;
            node.Entities.Add(entity);
            entity.CurrentNode = node;
        }
    }

    #endregion

    #region Positions conversions

    public bool IsInRange(Vector2Int gridPos)
        => (gridPos.x >= 0 && gridPos.x < _mapSizeX) && (gridPos.y >= 0 && gridPos.y < _mapSizeY);

	/// <summary>
	/// Converts grid position to world position
	/// </summary>
	/// <param name="gridPos"></param>
	/// <returns></returns>
	public Vector3 ToWorldPos(Vector2Int gridPos) => new Vector3(gridPos.x, 0, gridPos.y) + _worldNodeOffset;

    /// <summary>
    /// Converts world position to grid position
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Vector2Int ToGridPos(Vector3 worldPos)
    {
        Vector3 position = worldPos - _worldNodeOffset;
        Vector3Int floored = Vector3Int.FloorToInt(position);

        return new Vector2Int(floored.x, floored.z);
    }

    /// <summary>
    /// Get node from grid position
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public Node GetNode(Vector2Int gridPos) => IsInRange(gridPos) ? _graph[gridPos.x, gridPos.y] : null;

    // <summary>
    /// Get node from world position
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public Node GetNode(Vector3 worldPos) => GetNode(ToGridPos(worldPos));

    /// <summary>
    /// Get nodes within bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public List<Node> GetNodes(Bounds bounds)
	{
        List<Node> result = new List<Node>();

		foreach (Node node in _graph)
		{
            if (bounds.Contains(node.WorldPos))
                result.Add(node);
		}
        return result;
	}

    public List<Node> GetFullRange(Node origin, int radius, bool onlyWalkables, List<Node> visited)
	{
        if (radius <= 0) return null;

        IEnumerable<Node> collection = origin.Neighbors;

        foreach (Node node in collection)
        {
            if (onlyWalkables && !node.IsWalkable) continue;
            if (!visited.Contains(node))
                visited.Add(node);
            GetFullRange(node, radius - 1, onlyWalkables, visited);
        }
        return visited;
    }

    private List<Node> GetEmptyRange(Node origin, int radius)
	{
        List<Node> result = new List<Node>();

        for (int i = 0; i < radius; i++)
        {
            result.Add(_graph[origin.Position.x + radius - i, origin.Position.y - i]);
            result.Add(_graph[origin.Position.x - radius + i, origin.Position.y + i]);
            result.Add(_graph[origin.Position.x + i, origin.Position.y + radius - i]);
            result.Add(_graph[origin.Position.x - i, origin.Position.y - radius + i]);
        }
        return result;
    }

    /// <summary>
    /// Returns a list of nodes that can be walked to given a number of movements
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<Node> GetWalkableSurroundings(Node origin, int movements)
	{
        List<Node> visited = new List<Node>();
        List<Node> result = GetFullRange(origin, movements, true, visited);

        if (result == null)
            return visited;
        result.Remove(origin);
        return result;
    }

    public List<Node> GetDamageableSurroundings(Node origin, int radius)
	{
		// TODO: uncomment the following block to test the GetLine() method

		//List<Node> emptyRange = GetEmptyRange(origin, radius);
		//List<Node> result = new List<Node>();

		//foreach (Node maxRange in emptyRange)
		//{
		//	List<Node> line = GetLine(origin, maxRange);

		//	line.Remove(origin);
		//	for (int i = 0; i < line.Count; i++)
		//	{
		//		if (!line[i].IsWalkable)
		//			break;
		//		result.Add(line[i]);
		//	}
		//}
		//return result;

		// TODO: comment this to test the GetLine() method
	    return GetFullRange(origin, radius, false, new List<Node>());
	}

    #endregion

    #region Entity manipulation

    /// <summary>
    /// Spawns an entity an adds it to the map
    /// </summary>
    /// <param name="entityPrefab"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public GameObject SpawnEntity(GameObject entityPrefab, Node node)
    {
        GameObject instance = Instantiate(entityPrefab.gameObject, ToWorldPos(node.Position), Quaternion.identity, _entityStorage);

        return AddEntity(instance, node);
    }

    public GameObject AddEntity(GameObject entityInstance, Node node)
	{
        EntityType type = entityInstance.GetComponent<EntityType>();

        node.Entities.Add(type);
        type.CurrentNode = node;
        if (type is ATurnBasedEntity)
            TurnBasedManager.Instance.AddEntity(type as ATurnBasedEntity);
        return entityInstance;
    }

    /// <summary>
    /// Delete an entity from the map
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveEntity(EntityType entity)
	{
		entity.CurrentNode.Entities.Remove(entity);
    }

    public void MoveEntity(Node origin, Node destination, EntityType entity)
	{
        origin.Entities.Remove(entity);
        destination.Entities.Add(entity);
        entity.CurrentNode = destination;
	}

    #endregion

    #region Pathfinding

    public List<Node> GetPath(Node origin, Node destination)
    {
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();

        dist[origin] = 0;
        prev[origin] = null;

        foreach (Node v in _graph)
        {
            if (v != origin)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }

        while (unvisited.Count > 0)
        {
            Node u = null;

            foreach (Node pu in unvisited)
            {
                if (u == null || dist[pu] < dist[u])
                    u = pu;
            }

            if (u == destination)
                break;
            unvisited.Remove(u);
            float alt;
            foreach (Node v in u.Neighbors)
            {
                if (v.IsWalkable)
                    alt = dist[u] + u.DistanceTo(v);
                else
                    alt = dist[u] + 9999999f;

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        if (prev[destination] == null)
            return null;

        List<Node> currentPath = new List<Node>();
        Node curr = destination;

        while (curr != null)
        {
            if (!curr.IsWalkable && curr != origin)
                currentPath.Clear();
            else
                currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();
		return currentPath;
    }
    #endregion

    #region Line of sight

    public List<Node> GetVisibleNodes(Node origin, int range)
    {
        List<Node> viewableNodes = GetFullRange(origin, range, false, new List<Node>());
        viewableNodes.Remove(origin);

        List<Node> visibleNodes = new List<Node>();

        RaycastHit hit;

        //avoids repeated method calls
        Vector3 originWC = origin.WorldPos;

        foreach (Node v in viewableNodes)
        {
            //avoids repeated method calls
            Vector3 destinationWC = v.WorldPos;
            Vector3 direction = destinationWC - originWC;
            float distanceToNode = Vector3.Distance(originWC, destinationWC);
            LayerMask mask = LayerMask.GetMask("Obstruction");

            //sends ray to target node, if it hits something before it reaches it, we should't be able to see the target node
            if (Physics.Raycast(originWC, direction, out hit, distanceToNode, mask))
            {
                UnityEngine.Debug.DrawRay(originWC, direction,Color.green,5,false);
                
                float distanceToHit = Vector3.Distance(originWC, hit.point);
                
                //checks if the object the ray is hitting is actually just in the target node, if it is we should be able to see the node
                if ((distanceToNode - distanceToHit) < 1.0f)
                {
                    visibleNodes.Add(v);
                }
            }
            else
            {
                visibleNodes.Add(v);
            }
        }
        return visibleNodes;
    }

    #endregion

    private void OnDrawGizmos()
	{
        if (!Application.isPlaying || !Generated) return;

        Gizmos.color = new Color(255, 0, 0, 0.3f);
        foreach (Node item in _graph)
            if (!item.IsWalkable)
                Gizmos.DrawCube(item.WorldPos, new Vector3(1, 0.1f, 1));

    }
}