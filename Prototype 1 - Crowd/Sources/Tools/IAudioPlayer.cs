using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioPlayer<T> where T : Enum
{
	void PlayAudio(T audioId, float volume);
}
