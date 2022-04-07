using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class VectorExtension
{
	public static Vector3 ToVector3XZ(this Vector2 vector, float y = 0) => new Vector3(vector.x, y, vector.y);
	public static Vector2 InverseXY(this Vector2 vector) => new Vector2(vector.y, vector.x);
	public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
}