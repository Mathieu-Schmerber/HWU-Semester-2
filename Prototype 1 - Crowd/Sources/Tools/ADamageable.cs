using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ADamageable : MonoBehaviour
{
    [SerializeField] private int _health;

    public void Damage(int amount)
	{
		_health -= amount;
		if (_health <= 0)
			OnDeath();
	}

	protected abstract void OnDeath();
}
