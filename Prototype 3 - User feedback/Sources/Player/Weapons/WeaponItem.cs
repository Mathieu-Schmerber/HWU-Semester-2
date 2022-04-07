using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
	private Vector3 _initialPos;
	private float _randomFloatingTime;

	private void OnEnable()
	{
		_initialPos = transform.position;
		_randomFloatingTime = Random.Range(0, 10);
		transform.Rotate(Vector3.up * Random.Range(0, 360));
	}

	private void Update()
	{
		transform.Rotate(Vector3.up * 3 * Time.deltaTime);
		transform.position = new Vector3(_initialPos.x, _initialPos.y + Mathf.Sin(Time.time + _randomFloatingTime) * 0.5f, _initialPos.z);
	}

	private void OnTriggerEnter(Collider other)
	{
		other.GetComponentInChildren<PlayerWeapon>()?.SuggestWeapon(this);
	}

	private void OnTriggerExit(Collider other)
	{
		other.GetComponentInChildren<PlayerWeapon>()?.UnSuggestWeapon(this);
	}
}