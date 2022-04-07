using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnAwake : MonoBehaviour
{
	[SerializeField] private AudioClip[] _clips;

	private void Start()
	{
		GetComponent<AudioSource>().PlayOneShot(_clips.Random());
	}
}
