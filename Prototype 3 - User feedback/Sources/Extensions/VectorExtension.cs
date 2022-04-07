using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Extends any type of UnityEngine.Vector
/// </summary>
public static class VectorExtension
{
	private static readonly Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

	public static Vector3 ToIsometric(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

	/// <summary>
	/// Converts a Vector2 to a Vector3, translating to X and Z
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector3 ToVector3XZ(this Vector2 vector, float y = 0) => new Vector3(vector.x, y, vector.y);
	public static Vector3 ToVector3XZ(this Vector2Int vector, float y = 0) => new Vector3(vector.x, y, vector.y);

	/// <summary>
	/// Converts a Vector2 to a Vector3, translating to X and Y
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector3 ToVector3XY(this Vector2 vector, float z = 0) => new Vector3(vector.x, vector.y, z);
	public static Vector3 ToVector3XY(this Vector2Int vector, float z = 0) => new Vector3(vector.x, vector.y, z);

	/// <summary>
	/// Returns the vector setting its X
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="x"></param>
	/// <returns></returns>
	public static Vector3 WithX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);

	/// <summary>
	/// Returns the vector setting its Y
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);

	/// <summary>
	/// Returns the vector setting its Z
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	public static Vector3 WithZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

	/// <summary>
	/// Switch a vector's X and Y
	/// </summary>
	/// <param name="vector"></param>
	/// <returns></returns>
	public static Vector2 InverseXY(this Vector2 vector) => new Vector2(vector.y, vector.x);
	public static Vector2 InverseXY(this Vector2Int vector) => new Vector2(vector.y, vector.x);
}