using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TurnBasedUI : MonoBehaviour
{
	[SerializeField] private GameObject _entityFramePrefab;
	[SerializeField] private GameObject _arrowPrefab;

	private List<Tuple<EntityFrame, GameObject>> _frameArrowPairs = new List<Tuple<EntityFrame, GameObject>>();

	#region Unity builtins

	private void OnEnable()
	{
		TurnBasedManager.Instance.OnEntityAdded += OnEntityAdded;
		TurnBasedManager.Instance.OnEntityRemoved += OnEntityRemoved;
	}

	private void OnDestroy()
	{
		if (TurnBasedManager.Instance == null)
			return;
		TurnBasedManager.Instance.OnEntityAdded -= OnEntityAdded;
		TurnBasedManager.Instance.OnEntityRemoved -= OnEntityRemoved;
	}

	#endregion

	/// <summary>
	/// Creates a EntityFrame and an Arrow next to it
	/// </summary>
	/// <param name="entity"></param>
	private void OnEntityAdded(ATurnBasedEntity entity)
	{
		GameObject arrow = _frameArrowPairs.Count > 0 ? Instantiate(_arrowPrefab, transform) : null;
		EntityFrame frame = Instantiate(_entityFramePrefab, transform).GetComponent<EntityFrame>();

		frame.SetEntity(entity);
		_frameArrowPairs.Add(new Tuple<EntityFrame, GameObject>(frame, arrow));
	}

	/// <summary>
	/// Removes the corresponding EntityFrame and Arrow 
	/// </summary>
	/// <param name="entity"></param>
	private void OnEntityRemoved(ATurnBasedEntity entity)
	{
		var entry = _frameArrowPairs.First(x => x.Item1.Entity.gameObject == entity.gameObject);

		if (entry.Item2 != null)
			Destroy(entry.Item2);
		Destroy(entry.Item1.gameObject);
		_frameArrowPairs.Remove(entry);
	}
}
