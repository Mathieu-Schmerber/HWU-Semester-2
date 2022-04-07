using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
	NORTH = 0,
	SOUTH = 1,
	EAST = 2,
	WEST = 3
}

/// <summary>
/// Representation of a room in the world layout grid
/// </summary>
public class RoomNode
{
	public struct Link
	{
		public RoomNode Node;
		public Direction Direction;
		public Vector2Int ExitPosition;

		public Link(RoomNode node, Direction direction, Vector2Int exitPosition)
		{
			Node = node;
			Direction = direction;
			ExitPosition = exitPosition;
		}
	}

	public int Xmin => Position.x;
	public int Xmax => Position.x + Size.x;
	public int Ymin => Position.y;
	public int Ymax => Position.y + Size.y;

	public Vector2Int Position { get; private set; }
	public Vector2Int Size { get; private set; }
	public Vector2 Center => Position + (Vector2)Size / 2f;

	public List<Link> Links { get; private set; } = new List<Link>();
	public Vector2Int[] Exits => Links.Select(x => x.ExitPosition - Position).ToArray();

	public RoomNode(Vector2Int position, Vector2Int size)
	{
		Position = position;
		Size = size;
	}

	#region Neigbourhood

	private float GetClosestCornerDistance(Vector2Int pos)
	{
		Vector2Int[] corners = new Vector2Int[4]
		{
			new Vector2Int(Xmin, Ymin), // Bottom left
			new Vector2Int(Xmin, Ymax), // Up left
			new Vector2Int(Xmax, Ymax), // Up right
			new Vector2Int(Xmax, Ymin)  // Bottom right
		};

		return corners.Select(x => Vector2Int.Distance(x, pos)).Min();
	}

	/// <summary>
	/// Get distance of overlap between two rooms.
	/// At this point we know that two rooms are touching each other.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	private RectInt GetOverlapArea(RoomNode node)
	{
		int[] xAxis = { Xmin, Xmax, node.Xmin, node.Xmax };
		int[] yAxis = { Ymin, Ymax, node.Ymin, node.Ymax };
		xAxis = xAxis.OrderBy(x => x).ToArray();
		yAxis = yAxis.OrderBy(y => y).ToArray();

		return new RectInt(xAxis[1], yAxis[1], xAxis[2] - xAxis[1], yAxis[2] - yAxis[1]);
	}

	private bool Contains(int posX, int posY) 
		=> (Ymin <= posY && posY <= Ymax) && (Xmin <= posX && posX <= Xmax);

	public bool Contains(RoomNode other, Vector2Int padding)
	{
		int bottomLeft = Contains(other.Xmin, other.Ymin) ? 1 : 0;
		int upLeft = Contains(other.Xmin, other.Ymax) ? 1 : 0;
		int upRight = Contains(other.Xmax, other.Ymax) ? 1 : 0;
		int bottomRight = Contains(other.Xmax, other.Ymin) ? 1 : 0;
		int nbCornerMatch = bottomLeft + bottomRight + upLeft + upRight;

		// Check if the inner part of the rects are still neighbours after applying padding
		if (nbCornerMatch == 1)
		{
			if (bottomLeft == 1 && GetClosestCornerDistance(new Vector2Int(other.Xmin, other.Ymin)) <= padding.x * 2)
				return false;
			if (upLeft == 1 && GetClosestCornerDistance(new Vector2Int(other.Xmin, other.Ymax)) <= padding.y * 2)
				return false;
			if (upRight == 1 && GetClosestCornerDistance(new Vector2Int(other.Xmax, other.Ymax)) <= padding.y * 2)
				return false;
			if (bottomRight == 1 && GetClosestCornerDistance(new Vector2Int(other.Xmax, other.Ymin)) <= padding.x * 2)
				return false;
		}
		return nbCornerMatch > 0;
	}

	/// <summary>
	/// Returns true if the given RoomNode is a neighbour, even after cutting the given innerPadding.
	/// </summary>
	/// <param name="other"></param>
	/// <param name="offset"></param>
	/// <returns></returns>
	public bool IsNeigbour(RoomNode other, Vector2Int innerPadding)
	{
		// Ignore diagonals
		if ((Xmin == other.Xmax && Ymin == other.Ymax) || // Bottom left
			(Xmax == other.Xmin && Ymax == other.Ymin) || // Up right
			(Xmin == other.Xmax && Ymax == other.Ymin) || // Up left
			(Xmax == other.Xmin && Ymin == other.Ymax))	  // Bottom right	
			return false;

		return this.Contains(other, innerPadding) || other.Contains(this, innerPadding); 
	}

	#endregion

	#region Linking

	private Direction GetOppositeDirection(Direction direction)
		=> ((int)direction % 2 == 0 ? direction + 1 : direction - 1);

	private Vector2Int GetRandomExit(RoomNode other, Direction direction, Vector2Int padding)
	{
		bool IsHorizontal = direction == Direction.NORTH || direction == Direction.SOUTH;
		RectInt area = GetOverlapArea(other);
		RectInt innerArea = new RectInt(
			new Vector2Int(IsHorizontal ? area.x + padding.x : area.x, IsHorizontal ? area.y : area.y + padding.y),
			new Vector2Int(IsHorizontal ? area.size.x - padding.x * 3 : area.size.x, IsHorizontal ? area.size.y : area.size.y - padding.y * 3)
			// I'm removing padding * 3 because (padding_right + padding_left + int_rounded)
		);
		Vector2Int randomPos = new Vector2Int(Random.Range(innerArea.xMin, innerArea.xMax + 1), Random.Range(innerArea.yMin, innerArea.yMax + 1));

		return randomPos;
	}

	private Vector2Int OffsetExit(Direction direction, Vector2Int exit)
	{
		int xInc = 0;
		int yInc = 0;

		switch (direction)
		{
			case Direction.SOUTH:
				yInc--;
				break;
			case Direction.EAST:
				xInc--;
				break;
		}
		return new Vector2Int(exit.x + xInc, exit.y + yInc);
	}

	public void SetLinks(IEnumerable<RoomNode> neighbours, Vector2Int padding)
	{
		foreach (RoomNode other in neighbours)
		{
			if (Links.Any(x => x.Node == other))
				continue;

			Direction direction;
			if (Ymin == other.Ymax)
				direction = Direction.NORTH;
			else if (Ymax == other.Ymin)
				direction = Direction.SOUTH;
			else if (Xmin == other.Xmax)
				direction = Direction.WEST;
			else
				direction = Direction.EAST;

			Vector2Int exit = GetRandomExit(other, direction, padding);
			Links.Add(new Link(other, direction, OffsetExit(direction, exit)));
			other.Links.Add(new Link(this, GetOppositeDirection(direction), OffsetExit(GetOppositeDirection(direction), exit)));
		}
	}

	#endregion

	/// <summary>
	/// Split the room on the X axis. 
	/// </summary>
	/// <returns>The created room from the split</returns>
	public RoomNode CutX(int cutLenght)
	{
		Vector2Int remainingLenght = new Vector2Int(Size.x - cutLenght, Size.y);

		Size = new Vector2Int(cutLenght, Size.y);
		return new RoomNode(Position + Vector2Int.right * cutLenght, remainingLenght);
	}

	/// <summary>
	/// Split the room on the Y axis. 
	/// </summary>
	/// <returns>The created room from the split</returns>
	public RoomNode CutY(int cutLenght)
	{
		Vector2Int remainingLenght = new Vector2Int(Size.x, Size.y - cutLenght);

		Size = new Vector2Int(Size.x, cutLenght);
		return new RoomNode(Position + Vector2Int.up * cutLenght, remainingLenght);
	}
}