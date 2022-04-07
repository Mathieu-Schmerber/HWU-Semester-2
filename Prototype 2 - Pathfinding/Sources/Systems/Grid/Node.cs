using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
	public List<Node> Neighbors;
	public List<EntityType> Entities;
	public Vector2Int Position;

	public Node()
	{
		Neighbors = new List<Node>();
		Entities = new List<EntityType>();
	}

	public float DistanceTo(Node n) => Vector2.Distance(Position, n.Position);

	public Vector3 WorldPos => EntityMap.Instance.ToWorldPos(Position);

	public bool IsWalkable => !Entities.Any(x => x.IsObstacle);

	public bool IsEmpty => Entities.Count == 0;
}