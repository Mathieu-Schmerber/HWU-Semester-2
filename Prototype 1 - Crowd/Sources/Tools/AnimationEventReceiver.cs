using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
	private IAnimationReceiver[] _receivers;

	private void Awake()
	{
		_receivers = GetComponentsInParent<IAnimationReceiver>();
	}

	public void OnAnimationEvent(string animationEvent)
	{
		foreach (IAnimationReceiver receiver in _receivers)
			receiver?.OnAnimationEvent(animationEvent);
	}
}
