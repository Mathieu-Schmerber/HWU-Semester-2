using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EntityType : MonoBehaviour
{
    [SerializeField] private bool _IsObstacle = true;

	public Node CurrentNode { get; set; }
	public bool IsObstacle => _IsObstacle;

	public virtual void OnEntityCross(ATurnBasedEntity entity) {}
}
