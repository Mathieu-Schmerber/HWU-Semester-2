using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToObject : MonoBehaviour
{
    private LineRenderer _lr;
	[SerializeField] private float yOffset;
	public Transform ObjectLink;

	private void Awake()
	{
		_lr = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		_lr.SetPosition(0, transform.position);
		_lr.SetPosition(1, ObjectLink.position + Vector3.up * yOffset);
	}
}
