using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	private AudioSource _source;

	private void Awake()
	{
		_source = GetComponent<AudioSource>();
	}

	public static void SetPitch(float pitch, float duration)
	{
		Tween.Pitch(Instance._source, Instance._source.pitch, pitch, duration, 0);
	}

	public static void SetVolume(float volume, float duration)
	{
		Tween.Volume(Instance._source, Instance._source.volume, volume, duration, 0);
	}
}