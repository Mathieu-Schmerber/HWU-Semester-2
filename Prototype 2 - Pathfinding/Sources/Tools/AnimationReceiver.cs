using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
	private IAnimationListener[] _listeners;

	private void Awake()
	{
		_listeners = GetComponentsInParent<IAnimationListener>();
	}

	public void OnAnimationEvent(string argument) => _listeners.ForEach(x => x.OnAnimationEvent(argument));
}