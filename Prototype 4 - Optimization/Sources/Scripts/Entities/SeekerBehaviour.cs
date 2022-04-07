using Nawlian.Lib.Extensions;
using UnityEngine;

public class SeekerBehaviour : AEnemyController
{
	protected override Vector2 GetMovementNormal() 
		=> Vector3.Slerp(transform.up, (_target.transform.position.ToVector2XY() - _rb.position).normalized, 0.3f).normalized;
}