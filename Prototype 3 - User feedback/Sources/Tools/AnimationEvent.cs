using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
	private List<IAnimationReceiver> _receivers;

	private void Awake()
	{
		_receivers = GetComponentsInParent<IAnimationReceiver>().ToList();
		_receivers.AddRange(GetComponentsInChildren<IAnimationReceiver>());
	}

	public void OnAnimationEvent(string animation) => _receivers.ForEach(x => x.OnAnimationEvent(animation));
	public void OnAnimationEnter(AnimatorStateInfo stateInfo) => _receivers.ForEach(x => x.OnAnimationEnter(stateInfo));
	public void OnAnimationExit(AnimatorStateInfo stateInfo) => _receivers.ForEach(x => x.OnAnimationExit(stateInfo));
}
