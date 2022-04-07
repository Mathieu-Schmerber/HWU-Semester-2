using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IAnimationReceiver
{
	void OnAnimationEvent(string animationArg);
	void OnAnimationEnter(AnimatorStateInfo stateInfo);
	void OnAnimationExit(AnimatorStateInfo stateInfo);
}